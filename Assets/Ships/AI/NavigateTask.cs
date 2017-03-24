using System;
using System.Collections;
using UnityEngine;
using Pathfinding;

public class NavigateTask : AITask
{
    const float SHORT_THINK = 0.5f;
    const float LONG_THINK = 1.0f;

    [SerializeField]
    private Vector3 destination;

    private Coroutine navigate;

    public override Vector3? TargetLocation { get { return destination; } }

    public static NavigateTask Create(Vector3 dest)
    {
        var task = CreateInstance<NavigateTask>();
        task.destination = dest;

        return task;
    }

    public override void Begin()
    {
        navigate = null;        
    }

    public override void Update()
    {
        if (navigate == null)
        {
            var seeker = TaskFollower.GetComponent<Seeker>();

            if (seeker == null || !AstarPath.active || !AstarPath.active.isActiveAndEnabled)
            {
                TaskFollower.Ship.ResetControls(thrust: 1);
                TaskFollower.Ship.RotateToPoint(destination);
            }
            else
            {
                navigate = seeker.StartCoroutine(Navigate(seeker));
            }
        }
    }

    private IEnumerator Navigate(Seeker seeker)
    {
        while (!Done)
        {
            if (TaskFollower.Ship.CanSee(destination))
            {
                TaskFollower.Ship.ResetControls(thrust: 1);
                TaskFollower.Ship.RotateToPoint(destination);                
                
                yield return new WaitForSeconds(SHORT_THINK);
            }
            else
            {
                TaskFollower.Ship.ResetControls();
                yield return seeker.StartCoroutine(FollowWorldPath(seeker));
            }
        }
    }

    private IEnumerator FollowWorldPath(Seeker seeker)
    {
        //get a world path to target
        Debug.Log("getting a new world path...");
        var path = seeker.StartPath(TaskFollower.transform.position, destination, null, 1);
        yield return seeker.StartCoroutine(path.WaitForPath());

        if (path.error)
        {
            Debug.Log("world path failed");

            do
            {
                if (TaskFollower.Ship.CanSee(destination))
                {
                    TaskFollower.Ship.ResetControls(thrust: 1);
                    TaskFollower.Ship.RotateToPoint(destination);
                    yield return new WaitForSeconds(LONG_THINK);
                }
                else
                {
                    //feel our way forward blindly
                    var pos = TaskFollower.transform.position;
                    var between = destination - pos;

                    float lookAhead;
                    if (!GuessSizeOfObstacleInFront(between, out lookAhead))
                    {
                        lookAhead = TaskFollower.Ship.CurrentStats.MaxSpeed;
                    }

                    var aheadDest = pos + (between.normalized * lookAhead);

                    yield return seeker.StartCoroutine(FollowLocalPath(seeker, aheadDest));
                }                
            }
            while (!Done);
            
            yield break;
        }

        int pointIt = 0;

        do
        {
            //if at any point we can see the destination directly, go for that instead
            if (TaskFollower.Ship.CanSee(destination))
            {
                Debug.Log("giving up on world nav path because we can see the destination");
                TaskFollower.Ship.ResetControls(thrust: 1);
                TaskFollower.Ship.RotateToPoint(destination);
                yield return new WaitForSeconds(LONG_THINK);
                yield break;
            }

            var point = path.vectorPath[pointIt];
            bool lastPoint = pointIt == path.vectorPath.Count - 1;

            //can we see it?
            if (!TaskFollower.Ship.CanSee(point))
            {
                Debug.Log("can't see next world node, trying a local obstacle route");
                yield return seeker.StartCoroutine(FollowLocalPath(seeker, point));

                //start navigation again when we're done with that path
                yield break;
            }

            if (!lastPoint)
            {
                /* when doing world nav, skip a point if we can already see a clear path
                to the next point*/
                var nextPoint = path.vectorPath[pointIt + 1];
                if (TaskFollower.Ship.CanSee(nextPoint))
                {
                    ++pointIt;
                    continue;
                }
            }

            TaskFollower.Ship.ResetControls(thrust: 1);
            TaskFollower.Ship.RotateToPoint(point);

            if (TaskFollower.Ship.IsCloseTo(point))
            {
                ++pointIt;
            }
            else
            {
                yield return new WaitForSeconds(LONG_THINK);
            }
        }
        while (pointIt < path.vectorPath.Count);

        //if we reach the end of a world path and still can't see the target, look for a local obstacle path
        if (pointIt == path.vectorPath.Count
            && !TaskFollower.Ship.CanSee(destination))
        {
            Debug.Log("reached end of world path and couldn't see the dest, trying to navigate local obstacles");
            yield return seeker.StartCoroutine(FollowLocalPath(seeker, destination));
        }

        Debug.Log("stopped following a world route");
    }

    private IEnumerator FollowLocalPath(Seeker seeker, Vector3 worldDest)
    {
        var localPath = seeker.StartPath(TaskFollower.transform.position, worldDest, null, ~1);
        yield return seeker.StartCoroutine(localPath.WaitForPath());

        if (localPath.error)
        {
            Debug.Log("local path failed, going back to world nav");
            TaskFollower.Ship.ResetControls(thrust: 1);
            TaskFollower.Ship.RotateToPoint(worldDest);

            yield return new WaitForSeconds(LONG_THINK);
            yield break;
        }

        //follow the local obstacle's path
        int pointIt = 0;
        do
        {
            var point = localPath.vectorPath[pointIt];
            bool lastPoint = pointIt == localPath.vectorPath.Count - 1;

            TaskFollower.Ship.ResetControls(thrust: 1);
            TaskFollower.Ship.RotateToPoint(point, Vector3.up);
            
            if (!TaskFollower.Ship.CanSee(point))
            {
                Debug.Log("cancelling local route since we can't see the next node");
                yield return new WaitForSeconds(LONG_THINK);
                yield break;
            }

            if (!lastPoint)
            {
                /* skip points if we can already see the next point */
                var nextPoint = localPath.vectorPath[pointIt + 1];
                if (TaskFollower.Ship.CanSee(nextPoint))
                {
                    ++pointIt;
                    continue;
                }
            }

            if (TaskFollower.Ship.IsCloseTo(point))
            {
                ++pointIt;
                Debug.Log("reached a waypoint on our local route");
            }
            else
            {
                //Debug.Log("flying on local route...");
                yield return new WaitForSeconds(SHORT_THINK);
            }
        }
        while (pointIt < localPath.vectorPath.Count);

        Debug.Log("stopped following a local route");
    }

    private bool GuessSizeOfObstacleInFront(Vector3 between, out float size)
    {
        var xform = TaskFollower.transform;
        var ray = new Ray(xform.position, between);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var diameter = hit.collider.bounds.extents.magnitude * 2;
            size = diameter;

            return true;
        }

        size = 0;
        return false;
    }

    public override void End()
    {
        if (navigate != null)
        {
            TaskFollower.GetComponent<Seeker>().StopCoroutine(navigate);
        }

        //stop moving
        Ship.ResetControls();

        base.End();
    }

    public override bool Done
    {
        get
        {
            Debug.Assert(TaskFollower && TaskFollower.Ship, "should have a ship when checking done, don't check Done on same frame as follower is created!");

            return TaskFollower.Ship.IsCloseTo(destination);
        }
    }
}
