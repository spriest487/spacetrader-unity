using UnityEngine;

public abstract class AITask : ScriptableObject
{
    [SerializeField]
    private AITaskFollower taskFollower;

    private bool inProgress = false;

    public abstract bool Done { get; }

    public bool InProgress
    {
        get { return inProgress; }
        set { inProgress = value; }
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
