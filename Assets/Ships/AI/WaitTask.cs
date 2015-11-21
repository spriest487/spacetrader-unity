using UnityEngine;

public class WaitTask : AITask
{
    [SerializeField]
    private float duration;

    [SerializeField]
    private float started;

    public static WaitTask Create(float duration)
    {
        WaitTask task = CreateInstance<WaitTask>();
        task.duration = duration;
        return task;
    }

    public override bool Done
    {
        get
        {
            return Time.time > started + duration;
        }
    }

    public override void Update()
    {
        TaskFollower.Captain.Throttle = 0;
    }

    public override void Begin()
    {
        started = Time.time;
    }
}
