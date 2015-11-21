using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
#if UNITY_EDITOR
using System;
using System.Reflection;
#endif

[RequireComponent(typeof(AICaptain))]
public class AITaskFollower : MonoBehaviour, ISerializationCallbackReceiver
{
    private LinkedList<AITask> tasks;
    
    [SerializeField]
    private AITask[] serializedTasks;

    [SerializeField]
    private AICaptain captain;

    public AICaptain Captain
    {
        get
        {
            return captain;
        }
    }

    public bool Idle
    {
        get
        {
            return tasks.Count == 0;
        }
    }

    public void OnBeforeSerialize()
    {
        if (tasks != null)
        {
            serializedTasks = new AITask[tasks.Count];
            tasks.CopyTo(serializedTasks, 0);
        }
        else
        {
            serializedTasks = new AITask[0];
        }
    }

    public void OnAfterDeserialize()
    {
        tasks = new LinkedList<AITask>(serializedTasks);
    }

#if UNITY_EDITOR
    void CheckRequiredTypes(AITask task)
    {
        object[] requireAttrs = task.GetType().GetCustomAttributes(typeof(RequireComponent), true);
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
                    if (GetComponent(requiredComponent) == null)
                    {
                        throw new UnityException("missing required component " +requiredComponent +" for task type " +task.GetType());
                    }
                }
            }
        }
    }
#endif

    public void AssignTask(AITask task)
    {
#if UNITY_EDITOR
        CheckRequiredTypes(task);
#endif
        task.TaskFollower = this;
        tasks.AddLast(task);
    }

    public void ClearTasks()
    {
        while (tasks.Last != null)
        {
            tasks.Last.Value.End();
            tasks.RemoveLast();
        }
    }

    void Start()
    {
        tasks = new LinkedList<AITask>();
        captain = GetComponent<AICaptain>();
    }
    
    void Update()
    {
        while(tasks.Count != 0)
        {
            var nextTask = tasks.Last.Value;

            if (!nextTask.InProgress)
            {
                nextTask.InProgress = true;
                nextTask.Begin();
            }

            nextTask.Update();

            if (nextTask.Done)
            {
                nextTask.End();
                tasks.RemoveLast();
            }
            else
            {
                /* stop updating tasks as soon as we hit one
                that isn't Done */
                break;
            }
        }
    }
}
