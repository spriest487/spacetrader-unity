using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AITaskFollower), typeof(AICaptain))]
public class CombatCaptain : MonoBehaviour
{
    private AITaskFollower taskFollower;
    
    //private Ship formationLeader;

    private void Start()
    {
        taskFollower = GetComponent<AITaskFollower>();
    }

    private void Update()
    {
        if (taskFollower.Idle)
        {
            var target = taskFollower.Ship.Target;

            if (target)
            {
                taskFollower.AssignTask(AttackTask.Create(target));
            }

            /*taskFollower.AssignTask(WaitTask.Create(2));
            taskFollower.AssignTask(NavigateTask.Create(new Vector3(50, 30, 50)));
            taskFollower.AssignTask(WaitTask.Create(2));
            taskFollower.AssignTask(NavigateTask.Create(new Vector3(-50, -30, -50)));*/
        }
    }
}
