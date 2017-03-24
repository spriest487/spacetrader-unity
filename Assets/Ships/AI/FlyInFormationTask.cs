using System;
using UnityEngine;

public class FlyInFormationTask : AITask
{
    const float FORMATION_MATCH_ANGLE = 15;

    private Ship leader;

    public static FlyInFormationTask Create(Ship leader)
    {
        var task = CreateInstance<FlyInFormationTask>();
        task.leader = leader;
        return task;
    }

    public override bool Done
    {
        get
        {
            //this task doesn't end unless it's cancelled or leader dies
            return !leader;
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
        var leaderVelocity = leader.RigidBody.velocity;

        if (leader.RigidBody.velocity.sqrMagnitude > Vector3.kEpsilon)
        {
            var dotToHeading = Vector3.Dot(leaderVelocity.normalized, leader.RigidBody.transform.forward);
            angleDiffBetweenHeadings = Mathf.Acos(dotToHeading) * Mathf.Rad2Deg;
        }
        else
        {
            angleDiffBetweenHeadings = 0;
        }
        
        if (angleDiffBetweenHeadings < FORMATION_MATCH_ANGLE)
        {
            return Ship.EquivalentThrust(ship, leader);
        }
        else
        {
            return 0;
        }
    }

    public override void Update()
    {
        Debug.Assert(leader != Ship, "can't fly in formation with myself");
        
        /* if we're not close to the leader, fly towards them first */
        var fleet = Universe.FleetManager.GetFleetOf(Ship);
        if (fleet && fleet.Leader == leader)
        {
            var formationPos = fleet.GetFormationPos(Ship);

            if (!Ship.IsCloseTo(formationPos))
            {
                TaskFollower.AssignTask(NavigateTask.Create(formationPos));
                return;
            }
        }
        else
        {
            var safeDist = (leader.CloseDistance + Ship.CloseDistance) * 2;

            if (!Ship.IsCloseTo(leader.transform.position, safeDist))
            {
                TaskFollower.AssignTask(NavigateTask.Create(leader.transform.position));
                return;
            }
        }

        //fly in same direction as leader
        var dest = TaskFollower.transform.position + (leader.transform.forward * Ship.CurrentStats.MaxSpeed);

        float throttle = MatchLeaderThrust();

        Ship.ResetControls(thrust: throttle);
        Ship.RotateToDirection((dest - Ship.transform.position).normalized, leader.transform.up);        
    }
}
