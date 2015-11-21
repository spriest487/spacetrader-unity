using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class NavigateTask : AITask
{
    const float PATH_CHECK_INTERVAL = 1;

    [SerializeField]
    private Vector3 dest;

    [SerializeField]
    private Path path;

    [SerializeField]
    private float lastPathTime;

    private Seeker seeker;

    public static NavigateTask Create(Vector3 dest)
    {
        NavigateTask task = CreateInstance<NavigateTask>();
        task.dest = dest;
        return task;
    }

    private void PathCallback(Path newPath)
    {
        path = newPath;
        lastPathTime = Time.time;

        if (!newPath.error)
        {
            TaskFollower.AssignTask(FlyToPointTask.Create(dest));

            for (int i = path.vectorPath.Count - 1; i >= 0; --i)
            {
                TaskFollower.AssignTask(FlyToPointTask.Create(path.vectorPath[i]));
            }
        }
    }

    public override void Update()
    {
        if (path == null || Time.time > lastPathTime + PATH_CHECK_INTERVAL)
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
