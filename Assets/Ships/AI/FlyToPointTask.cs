using UnityEngine;

public class FlyToPointTask : AITask
{
    public static FlyToPointTask Create(Vector3 dest, float accuracy)
    {
        FlyToPointTask task = CreateInstance<FlyToPointTask>();
        task.dest = dest;
        task.accuracy = accuracy;
        return task;
    }

    [SerializeField]
    private Vector3 dest;

    [SerializeField]
    private float accuracy;

    public override bool Done
    {
        get
        {
            var toTarget = dest - TaskFollower.transform.position;
            var sqrAccuracy = (accuracy * accuracy);
            return toTarget.sqrMagnitude < sqrAccuracy;
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
