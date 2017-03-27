using System;
using UnityEngine;

public class FlyInFormationTask : AITask
{
    const float FormationMatchAngle = 3;

    private Fleet fleet;

    public static FlyInFormationTask Create(Fleet fleet)
    {
        var task = CreateInstance<FlyInFormationTask>();
        task.fleet = fleet;
        Debug.Assert(fleet, "can only assign formation task to fleet member");
        return task;
    }

    public override bool Done
    {
        get
        {
            //this task doesn't end unless it's cancelled or ship leaves fleet
            return !fleet ||
                !fleet.IsMember(Ship) ||
                fleet.Leader == Ship;
        }
    }

    /// <summary>
    /// If leader is going in a very different direction to the one they're facing,
    /// cut the throttle - we only follow forwards movement
    /// </summary>
    private float MatchLeaderThrust()
    {
        var ship = TaskFollower.Ship;

        float angleDiffBetweenHeadings;
        var leaderVelocity = fleet.Leader.RigidBody.velocity;

        if (fleet.Leader.RigidBody.velocity.sqrMagnitude > Vector3.kEpsilon)
        {
            var dotToHeading = Vector3.Dot(leaderVelocity.normalized, fleet.Leader.RigidBody.transform.forward);
            angleDiffBetweenHeadings = Mathf.Acos(dotToHeading) * Mathf.Rad2Deg;
        }
        else
        {
            angleDiffBetweenHeadings = 0;
        }

        if (angleDiffBetweenHeadings < FormationMatchAngle)
        {
            return Ship.EquivalentThrust(ship, fleet.Leader);
        }
        else
        {
            return 0;
        }
    }

    public override void Update()
    {
        /* if we're not close to the leader, fly towards them first */
        var formationPos = fleet.GetFormationPos(Ship);

        if (!Ship.CanSee(formationPos))
        {
            TaskFollower.AssignTask(NavigateTask.Create(formationPos));
            return;
        }
    
        Ship.PreciseManeuverTo(formationPos);
        
        if (Ship.IsCloseTo(formationPos))
        {
            Ship.RotateToDirection(fleet.Leader.transform.forward);

            float throttle = MatchLeaderThrust();
            Ship.ResetControls(Ship.Pitch, Ship.Yaw, Ship.Roll, Mathf.Min(throttle, Ship.Thrust), Ship.Strafe, Ship.Lift);
        }
    }
}
