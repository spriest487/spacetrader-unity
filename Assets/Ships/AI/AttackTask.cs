﻿#pragma warning disable 0649

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackTask : AITask
{ 
//{
//    private const float MINIMUM_THRUST = 0.25f;
//    private const float FOLLOW_RANGE = 50;
//    private const float ANGLE_MATCH = 0.8f; //TODO: get from weapons?
    
    //[SerializeField]
    //private bool behind;
    
    //[SerializeField]
    //private bool aimCloseEnough;

    //[SerializeField]
    //private bool canSeeTarget;

    //[SerializeField]
    //private bool evasive;
    
    [SerializeField]
    private Ship targetShip;

    bool done;
    bool collided;

    private Coroutine attackRoutine;

    public static AttackTask Create(Targetable target)
    {
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
            while (true)
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

                for (int mod = 0; mod < Ship.ModuleLoadout.SlotCount; ++mod)
                {
                    var module = Ship.ModuleLoadout.GetSlot(mod);
                    module.Aim = targetShip.transform.position;
                    module.Activate(Ship, mod);
                }

                yield return null;
            }

            //reposition
            Vector3 newPos;

            do
            {
                Ship.ResetControls(thrust: 0.33f);
                newPos = targetShip.transform.position + (Random.onUnitSphere * repositionDistance);
                yield return null;
            }
            while (!Ship.CanSee(newPos) || Ship.IsCloseTo(newPos));

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
