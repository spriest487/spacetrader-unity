using UnityEngine;
using System.Collections;

public class FlyToPointTask : AITask
{
    public static FlyToPointTask Create(Vector3 dest, float accuracy)
    {
        FlyToPointTask task = CreateInstance<FlyToPointTask>();
        task.dest = dest;
        task.accuracy = accuracy;
        return task;
    }

    public static FlyToPointTask Create(Ship dest, float accuracy)
    {
        var task = CreateInstance<FlyToPointTask>();
        task.dest = dest.transform.position;
        task.accuracy = dest.CloseDistance;
        return task;
    }

    [SerializeField]
    private Vector3 dest;

    [SerializeField]
    private float accuracy;

    private Coroutine flyRoutine;
    
    public override bool Done
    {
        get
        {
            var toTarget = dest - TaskFollower.transform.position;
            var sqrAccuracy = (accuracy * accuracy);
            return toTarget.sqrMagnitude < sqrAccuracy;
        }
    }

    private IEnumerator FlyRoutine()
    {
        var checkInterval = new WaitForSeconds(.1f);
        var ship = TaskFollower.Ship;

        while (!Done)
        {
            var between = (dest - TaskFollower.transform.position).normalized;
            var dotToTarget = Vector3.Dot(TaskFollower.transform.forward, between);

            /* slow down if we're not facing where we want to go (slower the
            further away from the correct heading we are) */
            ship.Thrust = Mathf.Clamp01(dotToTarget);
            ship.RotateToDirection(between);

            yield return checkInterval;            
        }
    }

    public override void Update()
    {
        if (flyRoutine == null)
        {
            flyRoutine = TaskFollower.StartCoroutine(FlyRoutine());
        }
    }

    public override void End()
    {
        if (flyRoutine != null)
        {
            TaskFollower.StopCoroutine(flyRoutine);
            flyRoutine = null;
        }
    }
}
