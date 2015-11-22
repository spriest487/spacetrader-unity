using UnityEngine;

public abstract class AITask : ScriptableObject
{
    public enum TaskStatus
    {
        NEW,
        NOT_STARTED,
        IN_PROGRESS,
        FINISHED
    }

    [SerializeField]
    private AITaskFollower taskFollower;

    private TaskStatus status;

    public abstract bool Done { get; }

    public TaskStatus Status
    {
        get { return status; }
        set { status = value; }
    }

    public AITaskFollower TaskFollower
    {
        get { return taskFollower; }
        set { taskFollower = value; }
    }

    public virtual void Update()
    {
    }

    public virtual void Begin()
    {
    }

    public virtual void End()
    {
    }
}
