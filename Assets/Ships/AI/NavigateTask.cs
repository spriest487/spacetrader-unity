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

    private enum PathStatus
    {
        NOT_STARTED,
        SHORT_RANGE,
        LONG_RANGE,
        FAILED
    }

    [SerializeField]
    private PathStatus pathStatus;

    private Seeker seeker;

    private static bool graphMasksInit = false;
    private static int localGraphMask;
    private static int worldGraphMask;

    private static int FindGraphMask(string name)
    {
        foreach (var graph in AstarPath.active.graphs)
        {
            if (graph.name == name)
            {
                return 1 << (int) graph.graphIndex;
            }
        }

        throw new System.ArgumentException("graph doesn't exist: " +name);
    }

    private static void InitGraphMasks()
    {
        localGraphMask = FindGraphMask("Local");
        worldGraphMask = FindGraphMask("World");
        graphMasksInit = true;
    }

    private static int LocalGraphMask
    {
        get
        {
            if (!graphMasksInit)
            {
                InitGraphMasks();
            }
            return localGraphMask;
        }
    }

    private static int WorldGraphMask
    {
        get
        {
            if (!graphMasksInit)
            {
                InitGraphMasks();
            }
            return worldGraphMask;
        }
    }

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
        task.pathStatus = PathStatus.NOT_STARTED;
        return task;
    }

    private void PathCallback(Path newPath)
    {
        if (newPath.error)
        {
            path = null;

            if (pathStatus == PathStatus.SHORT_RANGE)
            {
                /* try the long-range nav graph (graph 1) */
                pathStatus = PathStatus.LONG_RANGE;
                seeker.StartPath(
                    start: TaskFollower.transform.position,
                    end: dest,
                    callback: null,
                    graphMask: WorldGraphMask);
                return;
            }
            else
            {
                pathStatus = PathStatus.FAILED;
                return;
            }
        }
        else
        {
            path = newPath;
            lastPathTime = Time.time;

            var closeDistance = TaskFollower.Captain.Ship.BaseStats.maxSpeed * Time.fixedDeltaTime;
            closeDistance += TaskFollower.Captain.CloseDistance;

            var closeAngle = 15.0f;
            
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
        if (pathStatus == PathStatus.NOT_STARTED)
        {
            /* here we assume a standard pathfinding setup with two separate
            graphs to deal with the fact that space is big! we should have the
            fine-grained nav (around obstacles) in graph ID 0, and the long-range
            world nav (between celestial objects) in graph ID 1.
            
            we start with the fine-grained one first then try the long-range one
            if we can't get a path */
            pathStatus = PathStatus.SHORT_RANGE;
            seeker.StartPath(
                start: TaskFollower.transform.position, 
                end: dest, 
                graphMask: LocalGraphMask, 
                callback: null);
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
            if (pathStatus == PathStatus.FAILED)
            {
                return true;
            }

            return TaskFollower.Captain.IsCloseTo(dest);
        }
    }
}
