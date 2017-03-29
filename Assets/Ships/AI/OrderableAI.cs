using UnityEngine;

[RequireComponent(typeof(AITaskFollower))]
public class OrderableAI : MonoBehaviour
{
    public AITaskFollower TaskFollower { get; private set; }
    public Ship Ship { get { return TaskFollower.Ship; } }
    
    public Vector3? FocusPoint { get; private set; }

    [SerializeField]
    private AIOrder activeOrder = AIOrder.Wait;

    public AIOrder ActiveOrder
    {
        get { return activeOrder; }
    }

    protected virtual void Awake()
    {
        TaskFollower = GetComponent<AITaskFollower>();
    }

    public void SetOrder(AIOrder order)
    {
        SetOrder(order, null);
    }

    public void SetOrder(AIOrder order, Vector3? focusPoint)
    {
        FocusPoint = focusPoint;
        activeOrder = order;

        //interrupt current thoughts to do this
        TaskFollower.ClearTasks();
    }

    protected virtual void Update()
    {
        if (ActiveOrder == AIOrder.Wait)
        {
            /* special case for idling - we shouldn't take any tasks in this state */
            Ship.ResetControls();
            if (FocusPoint.HasValue && !Ship.IsCloseTo(FocusPoint.Value))
            {
                Ship.RotateToPoint(FocusPoint.Value);
            }

            if (!TaskFollower.Idle)
            {
                TaskFollower.ClearTasks();
            }
        }
        else if (TaskFollower.Idle)
        {
            /* only queue up new tasks if we're idle */
            switch (ActiveOrder)
            {
                case AIOrder.Move:
                    if (FocusPoint.HasValue)
                    {
                        TaskFollower.AssignTask(NavigateTask.Create(FocusPoint.Value));
                    }
                    else
                    {
                        SetOrder(AIOrder.Wait);
                    }
                    break;
                case AIOrder.Attack:
                    if (Ship.Target)
                    {
                        TaskFollower.AssignTask(AttackTask.Create(Ship.Target));
                    }
                    else
                    {
                        //can't attack if target isn't present, so go back to waiting
                        SetOrder(AIOrder.Wait);
                    }
                    break;
                default:
                    Debug.LogErrorFormat("unhandled task {0}!", ActiveOrder.ToString());
                    break;
            }
        }
    }
}
