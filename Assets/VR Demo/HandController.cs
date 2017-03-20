#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using UnityEngine.VR;

public class HandController : MonoBehaviour
{
    const float WakeUpDist = 0.01f;

    public enum HandControllerOrder
    {
        Move,
        Attack,
        Follow,
    }

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
    }

    private void OnEnable()
    {
        lastPos = InputTracking.GetLocalPosition(node);
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

    private void OnDisable()
    {
        dragging = null;
    }
    
    private IEnumerator DragShipMovement(Ship ship)
    {
        var issuedOrder = HandControllerOrder.Move;

        while (TriggerAxisValue > 0 && ship)
        {
            if (Hotspot.TouchingShip && Hotspot.TouchingShip != ship)
            {
                switch (Targetable.Relationship(ship.Targetable, Hotspot.TouchingShip.Targetable))
                {
                    case TargetRelationship.Friendly:
                    case TargetRelationship.FleetMember:
                        issuedOrder = HandControllerOrder.Follow;
                        break;
                    default:
                        issuedOrder = HandControllerOrder.Attack;
                        break;
                }
            }
            else
            {
                issuedOrder = HandControllerOrder.Move;
            }

            moveLine.gameObject.SetActive(true);
            var from = ship.transform.position;
            var to = Hotspot.transform.position;

            moveLine.SetPosition(0, from);
            moveLine.SetPosition(1, to);

            var dist = (from - to).magnitude;

            moveLine.colorGradient = OrderLineColor(issuedOrder);

            yield return null;
        }

        if (ship)
        {
            //dragged to empty space
            var tasks = ship.GetComponent<AITaskFollower>();
            if (tasks)
            {
                tasks.ClearTasks();

                switch (issuedOrder)
                {
                    case HandControllerOrder.Follow:
                        if (!Hotspot.TouchingShip)
                        {
                            goto default;
                        }
                        tasks.AssignTask(FlyInFormationTask.Create(Hotspot.TouchingShip));
                        break;
                    case HandControllerOrder.Attack:
                        if (!(Hotspot.TouchingShip && Hotspot.TouchingShip.Targetable))
                        {
                            goto default;
                        }
                        tasks.AssignTask(AttackTask.Create(Hotspot.TouchingShip.Targetable));
                        break;
                    default:
                        tasks.AssignTask(FlyToPointTask.Create(Hotspot.transform.position, Hotspot.Size));
                        break;
                }

                IssueCombatAIOrder(ship, issuedOrder);
            }
        }

        moveLine.gameObject.SetActive(false);
        dragging = null;
    }

    private Gradient OrderLineColor(HandControllerOrder order)
    {
        switch (order)
        {
            case HandControllerOrder.Attack: return attackLineGradient;
            case HandControllerOrder.Follow: return followLineGradient;
            default: return moveLineGradient;
        }
    }

    private void IssueCombatAIOrder(Ship ship, HandControllerOrder order)
    {
        var fleet = Universe.FleetManager.GetFleetOf(ship);
        if (fleet && fleet.Leader == ship)
        {
            WingmanOrder aiOrder;
            switch (order)
            {
                case HandControllerOrder.Attack: aiOrder = WingmanOrder.AttackLeaderTarget; break;
                case HandControllerOrder.Follow: aiOrder = WingmanOrder.FollowLeader; break;
                default: aiOrder = WingmanOrder.Wait; break;
            }

            if (aiOrder != WingmanOrder.Wait)
            {
                foreach (var follower in fleet.Followers)
                {
                    var combatAI = follower.GetComponent<WingmanCaptain>();
                    if (combatAI)
                    {
                        combatAI.SetOrder(aiOrder);
                    }
                }
            }
        }
    }
}
