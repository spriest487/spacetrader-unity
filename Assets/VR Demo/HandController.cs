#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using UnityEngine.VR;

public class HandController : MonoBehaviour
{
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

    private void Awake()
    {
        Hotspot = GetComponentInChildren<HandControllerHotspot>();
        room = GetComponentInParent<Room>();
    }

    private void Update ()
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
        while (TriggerAxisValue > 0 && ship)
        {
            moveLine.gameObject.SetActive(true);
            var from = ship.transform.position;
            var to = Hotspot.transform.position;

            moveLine.SetPosition(0, from);
            moveLine.SetPosition(1, to);

            var dist = (from - to).magnitude;
            
            if (Hotspot.TouchingShip && Hotspot.TouchingShip != ship)
            {
                switch (Targetable.Relationship(ship.Targetable, Hotspot.TouchingShip.Targetable))
                {
                    case TargetRelationship.Friendly:
                    case TargetRelationship.FleetMember:
                        moveLine.colorGradient = followLineGradient;
                        break;
                    default:
                        moveLine.colorGradient = attackLineGradient;
                        break;
                }
            }
            else
            {
                moveLine.colorGradient = moveLineGradient;
            }

            yield return null;
        }

        if (ship)
        {
            //dragged to empty space
            var tasks = ship.GetComponent<AITaskFollower>();
            if (tasks)
            {
                tasks.ClearTasks();

                if (Hotspot.TouchingShip && Hotspot.TouchingShip != ship)
                {
                    var relationship = Targetable.Relationship(ship.Targetable, Hotspot.TouchingShip.Targetable);

                    switch (relationship)
                    {
                        case TargetRelationship.Friendly:
                        case TargetRelationship.FleetMember:
                            tasks.AssignTask(FlyInFormationTask.Create(Hotspot.TouchingShip));
                            break;
                        default:
                            tasks.AssignTask(AttackTask.Create(Hotspot.TouchingShip.Targetable));
                            break;
                    }
                }
                else
                {
                    tasks.AssignTask(FlyToPointTask.Create(Hotspot.transform.position, Hotspot.Size));
                }
            }
        }

        moveLine.gameObject.SetActive(false);
        dragging = null;
    }
}
