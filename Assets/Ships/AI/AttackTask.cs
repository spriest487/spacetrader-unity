using System;
using UnityEngine;

public class AttackTask : AITask
{
    private const float MINIMUM_THRUST = 0.25f;
    private const float FOLLOW_RANGE = 50;
    private const float ANGLE_MATCH = 0.8f; //TODO: get from weapons?
    
    [SerializeField]
    private bool behind;
    
    [SerializeField]
    private bool aimCloseEnough;

    [SerializeField]
    private bool canSeeTarget;

    [SerializeField]
    private bool evasive;

    [SerializeField]
    private Targetable attackTarget;

    [SerializeField]
    private Ship attackShip;

    public static AttackTask Create(Targetable ship)
    {
        AttackTask task = CreateInstance<AttackTask>();
        task.attackTarget = ship;
        task.attackShip = ship.GetComponent<Ship>();
        return task;
    }

    public override void Update()
    {
        if (!attackTarget)
        {
            return;
        }
        
        canSeeTarget = TaskFollower.Ship.CanSee(attackTarget.transform.position);

        bool navigatingToTarget = false;
        if (!canSeeTarget)
        {
            /*there must be some obstacle in the way, 
            let's follow the nav points directly to them
            and see if that helps */
            TaskFollower.AssignTask(NavigateTask.Create(attackTarget.transform.position));
            navigatingToTarget = true;
        }

        if (!navigatingToTarget)
        {
            //are we aiming in the same direction as the target
            var dotToForward = Vector3.Dot(TaskFollower.transform.forward, attackTarget.transform.forward);
            aimCloseEnough = dotToForward > ANGLE_MATCH;

            //is my ship behind the Target
            var TargetToMe = (TaskFollower.transform.position - attackTarget.transform.position).normalized;
            var dotToMe = Vector3.Dot(attackTarget.transform.forward, TargetToMe);
            behind = dotToMe < 0;

            Vector3 dest = attackTarget.transform.position;

            if (evasive)
            {
                //try to get behind them
                var behindOffset = attackTarget.transform.forward - (attackTarget.transform.forward * FOLLOW_RANGE);
                dest += behindOffset;
            }

            if (TaskFollower.Ship.IsCloseTo(dest))
            {
                /* we're close to where we want to be, now aim at them */
                var thrust = attackShip? Ship.EquivalentThrust(TaskFollower.Ship, attackShip) : 0;
                TaskFollower.Ship.ResetControls(thrust: thrust);
                TaskFollower.Ship.RotateToPoint(attackTarget.transform.position);
            }
            else
            {
                //keep trying to get behind them
                TaskFollower.AssignTask(NavigateTask.Create(dest));

                //don't drop the speed too much in combat to keep dodging!
                //TaskFollower.Captain.MinimumThrust = MINIMUM_THRUST;
            }

            //pew pew pew
            TaskFollower.AssignTask(FireWeaponsTask.Create());
        }        
    }

    public override bool Done
    {
        get
        {
            if (!attackTarget)
            {
                //it's probably died
                return true;
            }

            return (behind || !evasive)
                && aimCloseEnough 
                && canSeeTarget;
        }
    }
}
