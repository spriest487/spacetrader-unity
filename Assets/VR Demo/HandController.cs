using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private VRNode node;

    [SerializeField]
    private Collider hotspotCollider;

    [SerializeField]
    private LineRenderer moveLine;

    public HandControllerHotspot Hotspot { get; private set; }
    
    private Coroutine dragging;

    private float TriggerAxisValue
    {
        get
        {
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

            yield return null;
        }

        if (ship)
        {
            var tasks = ship.GetComponent<AITaskFollower>();
            if (tasks)
            {
                tasks.ClearTasks();
                tasks.AssignTask(FlyToPointTask.Create(Hotspot.transform.position, Hotspot.Size));
            }
        }

        moveLine.gameObject.SetActive(false);
        dragging = null;
    }
}
