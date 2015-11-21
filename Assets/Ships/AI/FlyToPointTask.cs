using UnityEngine;

public class FlyToPointTask : AITask
{
    public static FlyToPointTask Create(Vector3 dest)
    {
        FlyToPointTask task = CreateInstance<FlyToPointTask>();
        task.dest = dest;
        return task;
    }

    [SerializeField]
    protected Vector3 dest;

    public override bool Done
    {
        get
        {
            return TaskFollower.Captain.IsCloseTo(dest);
        }
    }

    public override void Update()
    {
        var between = dest - TaskFollower.transform.position;
        var dotToTarget = Vector3.Dot(TaskFollower.transform.forward, between.normalized);

        /* slow down if we're not facing where we want to go (slower the
        further away from the correct heading we are) */
        TaskFollower.Captain.Throttle = Mathf.Clamp01(dotToTarget);

        TaskFollower.Captain.destination = dest;
        TaskFollower.Captain.adjustTarget = null;
        
        TaskFollower.Captain.MinimumThrust = 0;
    }
}
