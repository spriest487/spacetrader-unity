#pragma warning disable 0649

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;

//public enum HandControllerOrder
//{
//    None,
//    Move,
//    Attack,
//    Follow,
//}

public class HandController : MonoBehaviour
{
    const float WakeUpDist = 0.01f;
        
    [SerializeField]
    private VRNode node;
    
    [SerializeField]
    private LineRenderer moveLine;

    [SerializeField]
    private Gradient moveLineGradient;

    [SerializeField]
    private Gradient attackLineGradient;

    [SerializeField]
    private Gradient followLineGradient;

    public HandControllerHotspot Hotspot { get; private set; }
    public Canvas InfoPanel { get; private set; }

    /// <summary>
    /// order currently being issued to focus - player is dragging with the button,
    /// but hasn't released to issue the order yet
    /// </summary>
    public AIOrder PendingOrder { get; private set; }
    
    public Ship Focus { get; private set; }

    private Coroutine dragging;
    private Room room;

    private Vector3 lastPos;
    private float lastMoved;

    private float TriggerAxisValue
    {
        get
        {
            if (!VRSettings.enabled)
            {
                return Input.GetMouseButton(0) ? 1 : 0;
            }

            switch (node)
            {
                case VRNode.LeftHand: return Input.GetAxisRaw("OpenVR Trigger L");
                case VRNode.RightHand: return Input.GetAxisRaw("OpenVR Trigger R");
                default: return 0;
            }
        }
    }

    public bool HasMoved
    {
        get
        {
            if (!VRSettings.enabled)
            {
                return node == VRNode.RightHand;
            }

            const float WakeUpSqr = WakeUpDist * WakeUpDist;

            var trackedPos = InputTracking.GetLocalPosition(node);

            return (trackedPos - lastPos).sqrMagnitude > WakeUpSqr;
        }
    }

    public float IdleTime
    {
        get
        {
            return Time.time - lastMoved;
        }
    }

    private void Awake()
    {
        Hotspot = GetComponentInChildren<HandControllerHotspot>();
        room = GetComponentInParent<Room>();
        InfoPanel = GetComponentInChildren<Canvas>();
    }

    private void OnEnable()
    {
        lastPos = InputTracking.GetLocalPosition(node);
    }

    private void OnDisable()
    {
        dragging = null;
        Focus = null;
    }

    private void Update()
    {
        var pos = InputTracking.GetLocalPosition(node);
        var rot = InputTracking.GetLocalRotation(node);
        
        transform.localPosition = pos;
        transform.localRotation = rot;

        if (TriggerAxisValue > 0 && Hotspot.TouchingShip && dragging == null)
        {
            dragging = StartCoroutine(DragShipMovement(Hotspot.TouchingShip));
        }

        if (!VRSettings.enabled && node == VRNode.RightHand)
        {
            transform.rotation = Quaternion.identity;

            var mousePos = Input.mousePosition;
            var mouseRay = room.OverheadCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity))
            {
                AlignByHotspot(hit.point);
            }
            else
            {
                var worldPos = room.OverheadCamera.ScreenToWorldPoint(mousePos);
                worldPos.y = 0;
                AlignByHotspot(worldPos);
            }
        }

        InfoPanel.worldCamera = VRSettings.enabled ? room.VRCamera : room.OverheadCamera;

        if (HasMoved)
        {
            lastMoved = Time.time;
            lastPos = pos;
        }
	}

    private void AlignByHotspot(Vector3 point)
    {
        var offset = Hotspot.transform.position - transform.position;

        transform.position = point - offset;
    }
        
    private IEnumerator DragShipMovement(Ship focus)
    {
        Focus = focus;
        PendingOrder = AIOrder.Wait;
        
        while (TriggerAxisValue > 0 && Focus)
        {
            if (Hotspot.TouchingShip && Hotspot.TouchingShip != Focus)
            {
                switch (Targetable.Relationship(Focus.Targetable, Hotspot.TouchingShip.Targetable))
                {
                    case TargetRelationship.Friendly:
                        PendingOrder = AIOrder.Move;
                        break;
                    default:
                        PendingOrder = AIOrder.Attack;
                        break;
                }
            }
            else
            {
                PendingOrder = AIOrder.Move;
            }

            moveLine.gameObject.SetActive(true);
            var from = Focus.transform.position;
            var to = Hotspot.transform.position;

            moveLine.SetPosition(0, from);
            moveLine.SetPosition(1, to);

            var dist = (from - to).magnitude;

            Debug.Assert(OrderLineColor(PendingOrder) != null, "must have an order with a valid color when dragging");
            moveLine.colorGradient = OrderLineColor(PendingOrder);

            yield return null;
        }

        if (Focus)
        {
            //dragged to empty space
            var tasks = Focus.GetComponent<AITaskFollower>();
            if (tasks)
            {
                var fleets = Universe.FleetManager;

                /* issuing order to anyone except fleet leader causes them to 
                 leave the fleet to do their own thing */
                if (PendingOrder != AIOrder.Wait)
                {
                    var fleet = fleets.GetFleetOf(Focus);
                    if (fleet && fleet.Leader != Focus)
                    {
                        fleets.LeaveFleet(Focus);
                    }
                }
                
                tasks.ClearTasks();

                switch (PendingOrder)
                {
                    case AIOrder.Move:
                        if (!Hotspot.TouchingShip)
                        {
                            goto default;
                        }
                        /* join fleet of target ship */
                        var joinedFleet = fleets.AddToFleet(Hotspot.TouchingShip, Focus);
                        tasks.AssignTask(FlyInFormationTask.Create(joinedFleet));
                        break;
                    case AIOrder.Attack:
                        if (!(Hotspot.TouchingShip && Hotspot.TouchingShip.Targetable))
                        {
                            goto default;
                        }
                        tasks.AssignTask(AttackTask.Create(Hotspot.TouchingShip.Targetable));
                        break;
                    default:
                        tasks.AssignTask(NavigateTask.Create(Hotspot.transform.position));
                        break;
                }

                IssueCombatAIOrder(Focus, PendingOrder);
            }
        }

        moveLine.gameObject.SetActive(false);
        dragging = null;
        Focus = null;
        PendingOrder = AIOrder.Wait;
    }

    public Gradient OrderLineColor(AIOrder order)
    {
        switch (order)
        {
            case AIOrder.Attack: return attackLineGradient;
            case AIOrder.Move: return moveLineGradient;
            default: return null;
        }
    }

    private void IssueCombatAIOrder(Ship ship, AIOrder aiOrder)
    {
        var fleet = Universe.FleetManager.GetFleetOf(ship);
        if (fleet && fleet.Leader == ship)
        {
            if (aiOrder != AIOrder.Wait)
            {
                foreach (var follower in fleet.Followers)
                {
                    var combatAI = follower.GetComponent<CombatAI>();
                    if (combatAI)
                    {
                        combatAI.SetOrder(aiOrder);
                    }
                }
            }
        }
    }
}
