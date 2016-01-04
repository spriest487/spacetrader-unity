﻿using System;
using UnityEngine;

public class AimAtTask : AITask
{
    [SerializeField]
    private Vector3 dest;

    [SerializeField]
    private Vector3 aimAtPos;
    
    [SerializeField]
    private float accuracy;
    
    public static AimAtTask Create(Vector3 dest, Vector3 aimAtPos, float accuracy)
    {
        AimAtTask task = CreateInstance<AimAtTask>();
        task.dest = dest;
        task.aimAtPos = aimAtPos;
        task.accuracy = Mathf.Deg2Rad * accuracy;

        return task;
    }

    public override bool Done
    {
        get
        {
            var forward = TaskFollower.transform.forward;
            var between = aimAtPos - TaskFollower.transform.position;
            var dotToDest = Vector3.Dot(TaskFollower.transform.forward, between.normalized);

            var angleToDest = Mathf.Acos(dotToDest);

            return angleToDest < accuracy;
        }
    }

    public override void Update()
    {
        TaskFollower.Captain.adjustTarget = dest;
        TaskFollower.Captain.destination = aimAtPos;
        TaskFollower.Captain.Throttle = 0;
    }
}