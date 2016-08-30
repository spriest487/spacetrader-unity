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
        var ship = TaskFollower.Ship;

        //fly in same direction as leader
        var dest = TaskFollower.transform.position + (leader.transform.forward * ship.CurrentStats.MaxSpeed);

        float throttle = MatchLeaderThrust();

        ship.ResetControls(thrust: throttle);
        ship.RotateToDirection((dest - ship.transform.position).normalized, leader.transform.up);        
    }
}
