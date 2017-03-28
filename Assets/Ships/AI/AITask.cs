using UnityEngine;
using System.Linq;

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

    public Ship Ship
    {
        get { return TaskFollower.Ship; }
    }

    public virtual Vector3? TargetLocation { get { return null; } }

    public virtual void Update()
    {
    }

    public virtual void Begin()
    {
    }

    public virtual void End()
    {
    }

    public virtual void OnCollided(Collision collision)
    {
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void CheckRequiredConstraints()
    {
        var requireAttrs = GetType().GetCustomAttributes(typeof(RequireComponent), true);
        foreach (RequireComponent require in requireAttrs)
        {
            var requiredComponents = new[]
            {
                require.m_Type0,
                require.m_Type1,
                require.m_Type2
            };

            var missingComponents = requiredComponents
                .Where(rc => rc != null)
                .Where(rc => !taskFollower.GetComponent(rc))
                .ToList();

            Debug.AssertFormat(!missingComponents.Any(), 
                "missing required components [{0}] for task type {1}", 
                string.Join(", ", missingComponents.Select(c => c.Name).ToArray()), 
                GetType());
        }
    }
}
