#pragma warning disable 0649

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackTask : AITask
{
    [SerializeField]
    private Ship targetShip;

    bool done;
    bool collided;

    private Coroutine attackRoutine;

    public static AttackTask Create(Targetable target)
    {
        Debug.Assert(target, "must attack a valid target");

        var task = CreateInstance<AttackTask>();
        task.targetShip = target.GetComponent<Ship>();
        task.done = false;
        return task;
    }

    private IEnumerator AttackLoop()
    {
        while (targetShip && targetShip.Targetable)
        {
            //keep focused on this target
            Ship.Target = targetShip.Targetable;

            var repositionDistance = 40f + targetShip.CloseDistance + Ship.CloseDistance; //TODO
            var closeDistance = repositionDistance / 2;

            //attack run
            while (Ship.Target)
            {
                var canSee = Ship.CanSee(targetShip.transform.position);
                var distSqr = (Ship.transform.position - targetShip.transform.position).sqrMagnitude;

                if (distSqr < closeDistance * closeDistance ||
                    !canSee)
                {
                    break;
                }

                if (collided)
                {
                    collided = false;
                    break;
                }

                Ship.ResetControls(thrust: 1);
                Ship.RotateToPoint(targetShip.transform.position);

                //try to keep moving while fighting
                if (Ship.Thrust < 0.33f)
                {
                    Ship.ResetControls(Ship.Pitch, Ship.Yaw, Ship.Roll, 0.33f, 0, 0);
                }

                Ship.ActivateWeapons();

                yield return null;
            }
            
            if (!targetShip || !targetShip.Targetable)
            {
                //probably destroyed them! hooray
                break;
            }

            //reposition
            var newPos = targetShip.transform.position + (Random.onUnitSphere * repositionDistance);

            while (!Ship.CanSee(newPos) || Ship.IsCloseTo(newPos))
            {
                Ship.ResetControls(thrust: 0.33f);
                Ship.ActivateWeapons();
                yield return null;
            }

            while (!Ship.IsCloseTo(newPos))
            {
                if (collided)
                {
                    collided = false;
                    break;
                }

                Ship.RotateToPoint(newPos);

                //always fly max speed here, don't brake to turn
                Ship.ResetControls(Ship.Pitch, Ship.Yaw, Ship.Roll, thrust: 1);
                Ship.ActivateWeapons();

                yield return null;
            }
        }

        done = true;
        attackRoutine = null;
    }

    public override void OnCollided(Collision collision)
    {
        collided = true;
    }

    public override void Update()
    {
        if (attackRoutine == null && !done)
        {
            attackRoutine = Ship.StartCoroutine(AttackLoop());
        }
    }

    public override bool Done
    {
        get
        {
            return done;
        }
    }
}
