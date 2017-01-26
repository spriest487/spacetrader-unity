#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class FollowCamera : MonoBehaviour
{
    const float NORMAL_FOV = 60;
    const float CINEMATIC_FOV = 25;

    public static FollowCamera Current { get; private set; }

    public const float UI_DRAG_DELAY = 0.2f;

    public Camera Camera { get; private set; }

    public Vector2? DragInput
    {
        get { return dragInput; }
    }

    [SerializeField]
    private WorldMap worldMap;

    [SerializeField]
    private bool ignoreTranslation;

    [Header("Cockpit")]

    [SerializeField]
    private CockpitView cockpitCam;

    [SerializeField]
    private bool cockpitMode;

    [SerializeField]
    [HideInInspector]
    private ShipType cockpitShipType;

    [Header("Positioning")]
    [SerializeField]
	private Vector3 offset;

    [SerializeField]
    private Vector3 jumpCamOffset;

    [SerializeField]
    private float thrustOffset;

    [SerializeField]
    private float rotationOffset;

    [SerializeField]
    private AnimationCurve dragInputCurve;

	private Vector3 currentSpeedOffset;

    private Vector2? dragInput;

    private float lookPitchTarget;
    private float lookPitch;
    private float lookYawTarget;
    private float lookYaw;

    /// <summary>
    /// if following a ship in a jump, where did the jump start (or at
    /// least, where did we first see them jumping?)
    /// </summary>
    private Vector3? jumpOrigin;

    [SerializeField]
    private AnimationCurve lookSnapBackCurve;

    private Coroutine waitToDragOnGui = null;

    private int invisibleLayer;
    private int defaultLayer;

    [Header("VR")]
    [SerializeField]
    private Transform guiAnchor;

    [SerializeField]
    private Transform hmdPositionOffset;

    [SerializeField]
    private Transform hmdRotationOffset;

    public Transform WorldSpaceGUIAnchor { get { return guiAnchor; } }

    public event Action<Vector3, Quaternion> OnHMDReset;

	void Awake()
	{
        Camera = GetComponentInChildren<Camera>();

		currentSpeedOffset = Vector3.zero;

        invisibleLayer = LayerMask.NameToLayer("Invisible");
        defaultLayer = LayerMask.NameToLayer("Default");
    }

    void OnWorldMapVisibilityChanged(bool visible)
    {
        Camera.enabled = !visible;
    }

    void OnEnable()
    {
        Debug.Assert(!Current || Current == this);
        Current = this;

        worldMap.OnVisibilityChanged += OnWorldMapVisibilityChanged;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;

        worldMap.OnVisibilityChanged -= OnWorldMapVisibilityChanged;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene s1, Scene s2)
    {
        //reset cockpit
        if (cockpitCam)
        {
            Destroy(cockpitCam.gameObject);
        }
    }

    void DockedCam(PlayerShip player)
    {
        Camera.fieldOfView = NORMAL_FOV;
        transform.rotation = Quaternion.identity;
        transform.position = player.Dockable.DockedAtStation.transform.position;
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
        Camera.fieldOfView = NORMAL_FOV;

        transform.position = offset;

        if (!player)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        if (cockpitCam && cockpitMode)
        {
            transform.rotation = player.transform.rotation;
            transform.position = player.transform.position;
        }
        else
        {
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
        }
    }

    void AutoDockingCam(PlayerShip player, SpaceStation station)
    {
        Camera.fieldOfView = CINEMATIC_FOV;

        transform.position = station.DockingViewpoint;

        var look = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(look, station.transform.up);
    }

    void JumpCam(PlayerShip player)
    {
        if (!jumpOrigin.HasValue)
        {
            jumpOrigin = player.transform.position;
        }

        Camera.fieldOfView = CINEMATIC_FOV;

        var offset = player.transform.rotation * jumpCamOffset;
        transform.position = jumpOrigin.Value + offset;

        var look = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(look, player.transform.up);
    }

    void CutsceneCam(CutsceneCameraRig cutsceneCamRig)
    {
        Camera.fieldOfView = CINEMATIC_FOV;

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
        var playerShip = Universe.LocalPlayer;

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

    private void ResetHMD()
    {
        var position = -Camera.transform.localPosition;
        var rotation = Quaternion.Inverse(Camera.transform.localRotation);

        hmdPositionOffset.localPosition = position;
        hmdRotationOffset.localRotation = rotation;

        if (OnHMDReset != null)
        {
            OnHMDReset.Invoke(position, rotation);
        }

        PlayerNotifications.GameMessage("Reset HMD orientation");
    }

    private void Update()
    {
        if (VRSettings.enabled)
        {
            if (Input.GetButtonDown("Reset Orientation") &&
                !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                ResetHMD();
            }
        }
        else
        {
            hmdPositionOffset.localPosition = Vector3.zero;
            hmdRotationOffset.localRotation = Quaternion.identity;
        }

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

            if (Input.GetButtonDown("turn") && GUIController.Current.ActiveScreen == ScreenID.HUD)
            {
                if (waitToDragOnGui == null)
                {
                    waitToDragOnGui = StartCoroutine(WaitToDragOnGui());
                }
            }
        }
    }

    private void SetupCockpit(PlayerShip player, bool showCockpit)
    {
        /*if we have an existing cockpit view, and the player's cockpit type
          has changed, destroy the old one */
        if (cockpitCam && (!player || player.Ship.ShipType != cockpitShipType))
        {
            Destroy(cockpitCam.gameObject);
        }

        /* if we don't have a cockpit, create a new one of the right type */
        if (!cockpitCam && player && player.Ship.ShipType.HasCockpit)
        {
            cockpitCam = player.Ship.ShipType.CreateCockpit(this);
            cockpitCam.transform.SetParent(transform, false);
            cockpitShipType = player.Ship.ShipType;
        }

        if (player && showCockpit && cockpitCam && Camera.enabled)
        {
            cockpitCam.gameObject.SetActive(true);
            player.gameObject.SetLayerRecursive(invisibleLayer);
        }
        else
        {
            if (cockpitCam)
            {
                cockpitCam.gameObject.SetActive(false);
            }

            if (player)
            {
                player.gameObject.SetLayerRecursive(defaultLayer);
            }
        }
    }

    void LateUpdate()
    {
        var player = Universe.LocalPlayer;

        if (player && player.Ship)
        {
            var dockable = player.Dockable;
            if (dockable && dockable.State != DockingState.InSpace)
            {
                SetupCockpit(player, false);

                switch (dockable.State)
                {
                    case DockingState.AutoDocking:
                        AutoDockingCam(player, dockable.AutoDockingStation);
                        break;
                    case DockingState.Docked:
                        DockedCam(player);
                        break;
                }
            }
            else
            {
                if (player.Ship.JumpTarget)
                {
                    SetupCockpit(player, false);
                    JumpCam(player);
                }
                else
                {
                    jumpOrigin = null;

                    SetupCockpit(player, cockpitMode);
                    FlightCam(player);
                }
            }
        }
        else
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
	}

	void FixedUpdate()
	{
        var player = Universe.LocalPlayer;
		if (!player || !player.isActiveAndEnabled)
		{
			return;
		}

		var playerRb = player.GetComponent<Rigidbody>();
		var ship = player.GetComponent<Ship>();

		if (playerRb && ship)
		{
			var speed = -playerRb.velocity / Mathf.Max(1, ship.CurrentStats.MaxSpeed);
			var targetSpeedOffset = speed * thrustOffset;
            currentSpeedOffset = Vector3.MoveTowards(currentSpeedOffset, targetSpeedOffset, 0.1f);
		}
	}
}
