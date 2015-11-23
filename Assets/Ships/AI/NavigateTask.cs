using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class NavigateTask : AITask
{
    [SerializeField]
    private Vector3 dest;

    [SerializeField]
    private int maxPathLength;

    [SerializeField]
    private Path path;

    [SerializeField]
    private float lastPathTime;

    private Seeker seeker;

    /// <summary>
    /// Fly to a point, following the pathfinding grid as necessary
    /// </summary>
    /// <param name="dest">Destination point to move to</param>
    /// <param name="maxSteps">If >0, only use this number of
    /// steps in the path before stopping</param>
    /// <returns></returns>
    public static NavigateTask Create(Vector3 dest, int maxSteps = 0)
    {
        NavigateTask task = CreateInstance<NavigateTask>();
        task.dest = dest;
        task.maxPathLength = maxSteps;
        return task;
    }

    private void PathCallback(Path newPath)
    {
        path = newPath;
        lastPathTime = Time.time;

        var closeDistance = TaskFollower.Captain.Ship.BaseStats.maxSpeed * Time.fixedDeltaTime;
        closeDistance += TaskFollower.Captain.CloseDistance;

        var closeAngle = 15.0f;

        if (!newPath.error)
        {
            int pathLength = path.vectorPath.Count;

            int maxSteps = maxPathLength < 1 ? pathLength : maxPathLength;
            maxSteps = System.Math.Min(pathLength, maxSteps);

            /* if the maxsteps is less than the path length, skip
                steps after maxsteps */
            int skippedSteps = pathLength - maxSteps;

            /* move as far as it takes to get to the max length
            path, then stop */
            dest = newPath.vectorPath[maxSteps - 1];
            TaskFollower.AssignTask(FlyToPointTask.Create(dest, closeDistance));

            for (int step = 0; step < maxSteps; ++step)
            {
                var pathIndex = (pathLength - 1) - (step + skippedSteps);
                var pathPoint = path.vectorPath[pathIndex];

                /* for every point except the first one, aim at
                    the next point after arriving */
                if (step > 0)
                {
                    var nextPoint = path.vectorPath[pathIndex + 1];                    
                    TaskFollower.AssignTask(AimAtTask.Create(nextPoint, nextPoint, closeAngle));
                }

                /* for the final step, aim at the destination after moving */
                if (step == 0)
                {
                    TaskFollower.AssignTask(AimAtTask.Create(dest, dest, closeAngle));
                }
                                
                TaskFollower.AssignTask(FlyToPointTask.Create(pathPoint, closeDistance));
            }
        }
    }

    public override void Update()
    {
        if (path == null)
        {
            seeker.StartPath(TaskFollower.transform.position, dest);
        }
    }

    public override void Begin()
    {
        seeker = TaskFollower.GetComponent<Seeker>();
        seeker.pathCallback += PathCallback;
    }

    public override void End()
    {
        seeker.pathCallback -= PathCallback;
    }

    public override bool Done
    {
        get
        {
            if (path != null && path.error)
            {
                return true;
            }

            return TaskFollower.Captain.IsCloseTo(dest);
        }
    }
}
