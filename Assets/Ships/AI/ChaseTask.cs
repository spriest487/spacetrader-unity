using System;
using UnityEngine;

public class ChaseTask : FlyToPointTask
{
    private const float MINIMUM_THRUST = 0.25f;
    private const float FOLLOW_RANGE = 50;
    private const float ANGLE_MATCH = 0.8f; //TODO: get from weapons?

    [SerializeField]
    private Targetable target;

    [SerializeField]
    private bool behind;
    
    [SerializeField]
    private bool aimCloseEnough;

    public static ChaseTask Create(Targetable ship)
    {
        ChaseTask task = CreateInstance<ChaseTask>();
        task.target = ship;
        return task;
    }

    public override void Update()
    {        
        //are we aiming in the same direction as the target
        var dotToForward = Vector3.Dot(TaskFollower.transform.forward, target.transform.forward);
        aimCloseEnough = dotToForward > ANGLE_MATCH;

        //is my ship behind the target
        var targetToMe = (TaskFollower.transform.position - target.transform.position).normalized;
        var dotToMe = Vector3.Dot(target.transform.forward, targetToMe);
        behind = dotToMe < 0;
        
        //a point directly behind them some way
        var behindOffset = target.transform.forward - (target.transform.forward * FOLLOW_RANGE);
        dest = target.transform.position + behindOffset;

        if (TaskFollower.Captain.IsCloseTo(dest))
        {
            /* we're close to where we want to be, now aim at them */
            if (!aimCloseEnough)
            {
                TaskFollower.AssignTask(AimAtTask.Create(dest, target.transform.position));
            }
        }
        else
        {
            //keep trying to get behind them
            base.Update();

            //don't drop the speed too much in combat to keep dodging!
            TaskFollower.Captain.MinimumThrust = MINIMUM_THRUST;
        }
    }

    public override bool Done
    {
        get
        {
            return behind && aimCloseEnough;
        }
    }
}
