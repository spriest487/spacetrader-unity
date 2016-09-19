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
            TaskFollower.Ship.PreciseManeuverTo(target.transform.position);
        }
        else
        {
            activated = true;
        }
    }
}
