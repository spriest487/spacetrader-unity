using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AITaskFollower), typeof(AICaptain))]
public class CombatCaptain : MonoBehaviour
{
    private AITaskFollower taskFollower;

    [SerializeField]
    private Targetable target;

    //private Ship formationLeader;

    private void Start()
    {
        taskFollower = GetComponent<AITaskFollower>();
    }

    private void Update()
    {
        if (taskFollower.Idle)
        {
            if (target)
            {
                taskFollower.AssignTask(ChaseTask.Create(target));
            }

            /*taskFollower.AssignTask(WaitTask.Create(2));
            taskFollower.AssignTask(NavigateTask.Create(new Vector3(50, 30, 50)));
            taskFollower.AssignTask(WaitTask.Create(2));
            taskFollower.AssignTask(NavigateTask.Create(new Vector3(-50, -30, -50)));*/
        }
    }
}
