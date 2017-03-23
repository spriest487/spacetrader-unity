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

    public override Vector3? TargetLocation { get { return dest; } }

    private IEnumerator FlyRoutine()
    {
        var origin = TaskFollower.transform.position;

        var checkInterval = new WaitForSeconds(.1f);
        var ship = TaskFollower.Ship;

        while (!Done)
        {
            /* slow down if we're not facing where we want to go (slower the
            further away from the correct heading we are) */
            //ship.ResetControls(thrust: 1);
            ship.PreciseManeuverTo(dest);
            ship.RotateToPoint(dest + (dest - origin));

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

        TaskFollower.Ship.ResetControls();
    }
}
