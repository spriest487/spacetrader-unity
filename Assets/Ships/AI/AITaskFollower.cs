using UnityEngine;
using System.Collections.Generic;
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
    private AITask lastTask;

    [SerializeField]
    private AICaptain captain;

    public AICaptain Captain
    {
        get
        {
            return captain;
        }
    }

    public AITask LastTask
    {
        get
        {
            return lastTask;
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
        Debug.Assert(Captain, "can't assign tasks without a captain (don't assign tasks on same frame as follower was instantiated)");

#if UNITY_EDITOR
        CheckRequiredTypes(task);
#endif
        task.Status = AITask.TaskStatus.NEW;
        task.TaskFollower = this;
        tasks.AddLast(task);
    }

    public void ClearTasks()
    {
        LinkedListNode<AITask> last;
        while ((last = tasks.Last) != null)
        {
            last.Value.Status = AITask.TaskStatus.FINISHED;
            last.Value.End();
            tasks.RemoveLast();
        }
    }

    void Awake()
    {
        tasks = new LinkedList<AITask>();
    }

    void Start()
    {
        captain = GetComponent<AICaptain>();
    }
    
    void Update()
    {
        while (true)
        {
            /* a task execution can add new tasks itself, but during an update we
            skip those at the end of the list that are marked NEW */
            LinkedListNode<AITask> nextTaskNode = tasks.Last;
            while(nextTaskNode != null && nextTaskNode.Value.Status == AITask.TaskStatus.NEW)
            {
                nextTaskNode = nextTaskNode.Previous;
            }

            if (nextTaskNode == null)
            {
                //no tasks remaining that aren't NEW
                break;
            }

            var nextTask = nextTaskNode.Value;

            if (nextTask.Status == AITask.TaskStatus.NOT_STARTED)
            {
                nextTask.Status = AITask.TaskStatus.IN_PROGRESS;
                nextTask.Begin();
            }

            nextTask.Update();

            if (nextTask.Done)
            {
                nextTask.Status = AITask.TaskStatus.FINISHED;
                nextTask.End();
                tasks.Remove(nextTaskNode);

                lastTask = nextTask;
            }
            else
            {
                /* stop executing tasks as soon as there's at least one
                non-NEW task that isn't DONE after one update */
                break;
            }
        }

        /* change status of tasks added during this update so they're not
        skipped next update */
        foreach (var task in tasks)
        {
            if (task.Status == AITask.TaskStatus.NEW)
            {
                task.Status = AITask.TaskStatus.NOT_STARTED;
            }
        }
    }
}
