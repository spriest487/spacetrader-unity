#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class FollowCamera : MonoBehaviour
{
    public const float UI_DRAG_DELAY = 0.2f;

    public Camera Camera { get; private set; }

    public Vector2? DragInput
    {
        get { return dragInput; }
    }

    [SerializeField]
    private bool ignoreTranslation;

    [SerializeField]
	private Vector3 offset;

    [SerializeField]
    private float thrustOffset;

    [SerializeField]
    private float rotationOffset;
    
    ///world speeds multiplied by this to get the added amount of shake
    [SerializeField]
    private float shakeCollisionConversion;

    [SerializeField]
    private AnimationCurve dragInputCurve;

	private Vector3 currentSpeedOffset;
            
    private Vector2? dragInput;

    private float lookPitchTarget;
    private float lookPitch;
    private float lookYawTarget;
    private float lookYaw;

    [SerializeField]
    private AnimationCurve lookSnapBackCurve;

    private Coroutine waitToDragOnGui = null;
    
	void Start()
	{
        Camera = GetComponent<Camera>();

		currentSpeedOffset = Vector3.zero;
	}

    void DockedCam(PlayerShip player, SpaceStation station)
    {
        transform.rotation = Quaternion.identity;
        transform.position = station.transform.position;
        transform.position -= new Vector3(0, 0, 100);
    }
    
    Quaternion GetAngularVelocityOffset(PlayerShip player)
    {
        var rb = player.GetComponent<Rigidbody>();
        if (!rb)
        {
            return Quaternion.identity;
        }
        
        float rotationOffsetAmt = rotationOffset * Mathf.Rad2Deg;
        return Quaternion.Euler(rb.angularVelocity.x * rotationOffsetAmt,
            rb.angularVelocity.y * rotationOffsetAmt,
            rb.angularVelocity.z * rotationOffsetAmt);
    }

    void FlightCam(PlayerShip player)
    {
        transform.position = offset;

        if (!player)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        var forwardPoint = player.transform.position + player.transform.forward * 1000;
        var forwardDirection = (forwardPoint - player.transform.position).normalized;

        var shipRot = Quaternion.LookRotation(forwardDirection, player.transform.up);
        var avOffset = GetAngularVelocityOffset(player);
        shipRot = avOffset * shipRot;

        var shipPos = player.transform.position;

        var lookRot = Quaternion.Euler(new Vector3(lookPitch, lookYaw));
        lookRot = shipRot * lookRot;

        var lookMat = new Matrix4x4();
        lookMat.SetTRS(Vector3.zero, lookRot, Vector3.one);
        
        var lookOffset = lookMat.MultiplyPoint(offset);
        if (!ignoreTranslation)
        {
            lookOffset += shipPos + currentSpeedOffset;
        }

        transform.rotation = lookRot;
        transform.position = lookOffset;
        
        return;
    }

    void CutsceneCam(CutsceneCameraRig cutsceneCamRig)
    {
        transform.position = cutsceneCamRig.View;

        var lookDirection = (cutsceneCamRig.Focus - cutsceneCamRig.View).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
    
    private Vector2? FindTouchPos()
    {
        //mouse input takes priority!
        if (Input.GetMouseButton(0))
        {
            return Input.mousePosition;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                return touch.position;
            }
        }

        return null;
    }

    public Vector2? GetScreenAimPoint(Vector3 origin)
    {
        var worldAim = GetWorldAimPoint(origin);

        if (!worldAim.HasValue)
        {
            return null;
        }

        var screenPos = Camera.WorldToScreenPoint(worldAim.Value);
        return new Vector2(screenPos.x, screenPos.y);
    }
    
    public Vector3? GetWorldAimPoint(Vector3 origin)
    {
        var cursorPos = GetAimCursorPos();
        if (!cursorPos.HasValue)
        {
            return null;
        }

        const float AIM_DEPTH = 1000;
        
        var mousePos = new Vector3(cursorPos.Value.x, cursorPos.Value.y, AIM_DEPTH);
        var mouseRay = Camera.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit))
        {
            return hit.point;
        }
        else
        {
            return mouseRay.origin + (mouseRay.direction * AIM_DEPTH);
        }
    }
    
    private Vector2? GetAimCursorPos()
    {
        //decide whether to use touch or mouse
        var touchPos = FindTouchPos();

        if (touchPos.HasValue)
        {
            return touchPos;
        }
        else if (Input.mousePresent)
        {
            return Input.mousePosition;
        }
        else
        {
            return null;
        }
    }

    private void UpdateDrag()
    {
        var touchPos = FindTouchPos();
        var playerShip = PlayerShip.LocalPlayer;
        
        if (!touchPos.HasValue || !playerShip)
        {
            dragInput = Vector2.zero;
            return;
        }

        float screenDragX = touchPos.Value.x / Screen.width;
        float screenDragY = touchPos.Value.y / Screen.height;

        var eyePos = new Vector2(0.5f, 0.5f);

        var dragX = (screenDragX - eyePos.x) / eyePos.x;
        var dragY = -((screenDragY - eyePos.y) / eyePos.y);

        dragX = dragInputCurve.Evaluate(dragX) * Mathf.Sign(dragX);
        dragY = dragInputCurve.Evaluate(dragY) * Mathf.Sign(dragY);

        dragInput = new Vector2(dragX, dragY);
    }
    
    private IEnumerator WaitToDragOnGui()
    {
        var pointerData = new PointerEventData(EventSystem.current);
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            yield return new WaitForSeconds(UI_DRAG_DELAY);
        }

        while (Input.GetButton("turn"))
        {
            Cursor.lockState = CursorLockMode.Confined;

            UpdateDrag();

            yield return null;
        }

        Cursor.lockState = CursorLockMode.None;
        waitToDragOnGui = null;
        dragInput = null;
    }

    private void Update()
    {
        if (Input.GetButton("look") && dragInput.HasValue)
        {            
            float yawInput = Mathf.Clamp(dragInput.Value.x, -1, 1);
            float pitchInput = Mathf.Clamp(dragInput.Value.y, -1, 1);

            lookYawTarget = yawInput * 180;
            lookPitchTarget = pitchInput * 180;

            lookYaw = Mathf.MoveTowards(lookYaw, lookYawTarget, 720 * Time.deltaTime);
            lookPitch = Mathf.MoveTowards(lookPitch, lookPitchTarget, 720 * Time.deltaTime);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            lookYawTarget = 0;
            lookPitchTarget = 0;

            var snapBackYawSpeed = lookSnapBackCurve.Evaluate(lookYaw);
            var snapBackPitchSpeed = lookSnapBackCurve.Evaluate(lookPitch);

            lookYaw = Mathf.MoveTowardsAngle(lookYaw, lookYawTarget, snapBackYawSpeed * Time.deltaTime);
            lookPitch = Mathf.MoveTowardsAngle(lookPitch, lookPitchTarget, snapBackPitchSpeed * Time.deltaTime);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (Input.GetButtonDown("turn") && ScreenManager.Instance.ScreenID == ScreenID.None)
            {
                if (waitToDragOnGui == null)
                {
                    waitToDragOnGui = StartCoroutine(WaitToDragOnGui());
                }
            }
        }
    }

    void LateUpdate()
    {
        var cutsceneCamRig = ScreenManager.Instance.CurrentCutsceneCameraRig;
        if (cutsceneCamRig)
        {
            CutsceneCam(cutsceneCamRig);
        }
        else
        {
            var player = PlayerShip.LocalPlayer;

            if (player)
            {
                var moorable = player.GetComponent<Moorable>();
                if (moorable && moorable.Moored)
                {
                    DockedCam(player, moorable.SpaceStation);
                }
                else
                {
                    FlightCam(player);
                }
            }
            else
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
        }        
	}

	void FixedUpdate()
	{
        var player = PlayerShip.LocalPlayer;
		if (!player || !player.isActiveAndEnabled)
		{
			return;
		}
        
		var playerRb = player.GetComponent<Rigidbody>();
		var ship = player.GetComponent<Ship>();
		
		if (playerRb && ship)
		{
			var speed = -playerRb.velocity / Mathf.Max(1, ship.CurrentStats.maxSpeed);
			var targetSpeedOffset = speed * thrustOffset;
            currentSpeedOffset = Vector3.MoveTowards(currentSpeedOffset, targetSpeedOffset, 0.1f);
		}
	}
}
