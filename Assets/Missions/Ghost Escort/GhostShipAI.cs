using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AITaskFollower), typeof(Ship))]
public class GhostShipAI : MonoBehaviour
{
    [SerializeField]
    private Transform goal;
    public Transform Goal { get { return goal; } }
    
    public Ship Ship { get; private set; }

    private AITaskFollower tasks;

    private void Awake()
    {
        tasks = GetComponent<AITaskFollower>();
        Ship = GetComponent<Ship>();
    }

    private void Start()
    {
        tasks.AssignTask(NavigateTask.Create(goal.position));
    }
}
