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

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void CheckRequiredConstraints()
    {
        object[] requireAttrs = GetType().GetCustomAttributes(typeof(RequireComponent), true);
        foreach (var attr in requireAttrs)
        {
            var require = (RequireComponent)attr;
            var requiredComponents = new[]
            {
            require.m_Type0,
            require.m_Type1,
            require.m_Type2
        };

            foreach (var requiredComponent in requiredComponents)
            {
                if (requiredComponent != null)
                {
                    if (taskFollower.GetComponent(requiredComponent) == null)
                    {
                        throw new UnityException("missing required component " + requiredComponent + " for task type " + GetType());
                    }
                }
            }
        }
    }
}
