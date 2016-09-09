using System;
using UnityEngine;

public class ActivateTask : AITask
{
    [SerializeField]
    private bool activated;

    [SerializeField]
    private ActionOnActivate target;

    public static ActivateTask Create(ActionOnActivate target)
    {
        var task = CreateInstance<ActivateTask>();
        task.activated = false;
        task.target = target;

        return task;
    }

    public override bool Done
    {
        get
        {
            return activated;
        }
    }

    public override void Update()
    {
        if (!target.TryActivate(TaskFollower.Ship))
        {
            //dumb as rocks default behaviour - fly directly at target
            var targetShip = target.GetComponent<Ship>();
            var accuracy = TaskFollower.Ship.CloseDistance;

            FlyToPointTask flyToTarget;
            if (targetShip) 
            {
                flyToTarget = FlyToPointTask.Create(targetShip, accuracy);
            }
            else
            {
                flyToTarget = FlyToPointTask.Create(target.transform.position, accuracy);
            }

            TaskFollower.AssignTask(flyToTarget);
        }
        else
        {
            activated = true;
        }
    }
}
