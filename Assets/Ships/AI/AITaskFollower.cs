﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Ship))]
public class AITaskFollower : MonoBehaviour, ISerializationCallbackReceiver
{
    private LinkedList<AITask> tasks;
    
    [SerializeField]
    private AITask[] serializedTasks;

    [SerializeField]
    private AITask lastTask;

    [SerializeField]
    private Ship ship;

    public Ship Ship
    {
        get
        {
            return ship;
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
    
    public void AssignTask(AITask task)
    {
        Debug.Assert(ship, "can't assign tasks without a ship (don't assign tasks on same frame as follower was instantiated)");

        task.CheckRequiredConstraints();
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
            Destroy(last.Value);
            tasks.RemoveLast();
        }
    }

    /// <summary>
    /// cancel a task and every task above it in the stack
    /// </summary>
    public void CancelTask(AITask cancelled)
    {
        Debug.Assert(!!cancelled, "cancelled task must exist");
        Debug.Assert(tasks.Contains(cancelled), "can't cancel a task we don't have");

        var last = tasks.Last;

        do
        {
            last.Value.Status = AITask.TaskStatus.FINISHED;
            last.Value.End();
            Destroy(last.Value);
            tasks.RemoveLast();
        }
        while ((last = tasks.Last) != null);
    }

    void Awake()
    {
        tasks = new LinkedList<AITask>();
    }

    void Start()
    {
        ship = GetComponent<Ship>();
    }
    
    void Update()
    {
        //ai doesn't run while jumping
        if (ship.JumpTarget)
        {
            return;
        }

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
                Destroy(nextTask);
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
