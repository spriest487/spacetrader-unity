#pragma warning disable 0649

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
    private Ship attackShip;

    public static AttackTask Create(Targetable ship)
    {
        AttackTask task = CreateInstance<AttackTask>();
        task.attackShip = ship.GetComponent<Ship>();
        return task;
    }

    public override void Update()
    {
        if (!attackShip || !attackShip.Targetable)
        {
            return;
        }

        Ship.Target = attackShip.Targetable;
        var fleet = Universe.FleetManager.GetFleetOf(Ship);
        if (fleet && fleet.Leader == Ship)
        {
            /* everyone else attack too */
            foreach (var lackey in fleet.Followers)
            {
                Ship.SendRadioMessage(RadioMessageType.Attack, lackey);
            }
        }

        canSeeTarget = Ship.CanSee(attackShip.transform.position);

        bool navigatingToTarget = false;
        if (!canSeeTarget)
        {
            /*there must be some obstacle in the way, 
            let's follow the nav points directly to them
            and see if that helps */
            TaskFollower.AssignTask(NavigateTask.Create(attackShip.transform.position));
            navigatingToTarget = true;
        }

        if (!navigatingToTarget)
        {
            //are we aiming in the same direction as the target
            var dotToForward = Vector3.Dot(TaskFollower.transform.forward, attackShip.transform.forward);
            aimCloseEnough = dotToForward > ANGLE_MATCH;

            //is my ship behind the Target
            var TargetToMe = (TaskFollower.transform.position - attackShip.transform.position).normalized;
            var dotToMe = Vector3.Dot(attackShip.transform.forward, TargetToMe);
            behind = dotToMe < 0;

            Vector3 dest = attackShip.transform.position;

            if (evasive)
            {
                //try to get behind them
                var behindOffset = attackShip.transform.forward - (attackShip.transform.forward * FOLLOW_RANGE);
                dest += behindOffset;
            }

            if (TaskFollower.Ship.IsCloseTo(dest))
            {
                /* we're close to where we want to be, now aim at them */
                var thrust = attackShip? Ship.EquivalentThrust(TaskFollower.Ship, attackShip) : 0;
                Ship.ResetControls(thrust: thrust);
                Ship.RotateToPoint(attackShip.transform.position);
            }
            else
            {
                //keep trying to get behind them
                TaskFollower.AssignTask(NavigateTask.Create(dest));

                //don't drop the speed too much in combat to keep dodging!
                //TaskFollower.Captain.MinimumThrust = MINIMUM_THRUST;
            }

            //pew pew pew
            var loadout = Ship.ModuleLoadout;
            for (int module = 0; module < loadout.SlotCount; ++module)
            {
                loadout.GetSlot(module).Aim = attackShip.transform.position;
                loadout.Activate(Ship, module);
            }
        }        
    }

    public override bool Done
    {
        get
        {
            if (!attackShip || !attackShip.Targetable)
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
