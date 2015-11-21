using System;
using UnityEngine;

public class AimAtTask : AITask
{
    private const float AIM_ACCURACY = 5.0f;

    [SerializeField]
    private Vector3 dest;

    [SerializeField]
    private Vector3 aimAtPos;

    public static AimAtTask Create(Vector3 dest, Vector3 aimAtPos)
    {
        AimAtTask task = CreateInstance<AimAtTask>();
        task.dest = dest;
        task.aimAtPos = aimAtPos;

        return task;
    }

    public override bool Done
    {
        get
        {
            var forward = TaskFollower.transform.forward;
            var between = dest - TaskFollower.transform.position;
            var dotToDest = Vector3.Dot(TaskFollower.transform.forward, between.normalized);

            var angleToDest = Mathf.Rad2Deg * Mathf.Acos(dotToDest);

            return angleToDest > AIM_ACCURACY;
        }
    }

    public override void Update()
    {
        TaskFollower.Captain.adjustTarget = dest;
        TaskFollower.Captain.destination = aimAtPos;
        TaskFollower.Captain.Throttle = 0;
    }
}