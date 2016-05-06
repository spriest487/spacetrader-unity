﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class FollowCamera : MonoBehaviour
{
    public Camera Camera { get; private set; }

    public Vector2? DragInput
    {
        get { return dragInput; }
    }

	public bool ignoreTranslation;

	public Vector3 offset;
	public float thrustOffset;
	public float rotationOffset;
	
	public float shakeMax;
	public float shakeSpeed;

	///world speeds multiplied by this to get the added amount of shake
	public float shakeCollisionConversion; 

	private Vector3 currentSpeedOffset;

	private float shake;
	private Vector3 shakeAngles;

    [SerializeField]
    private float lookYaw;

    [SerializeField]
    private float lookPitch;
    
    private Vector2? dragInput;

    private Coroutine waitToDragOnGui = null;

	private void AddShake(float amount)
	{
		shake = Mathf.Clamp(shakeMax, 0, amount + shake);
	}

	public void NotifyPlayerCollision(Collision collision)
	{
		AddShake(collision.relativeVelocity.magnitude);
	}

	void Start()
	{
        Camera = GetComponent<Camera>();

		currentSpeedOffset = Vector3.zero;
		shakeAngles = new Vector3(UnityEngine.Random.value * 2 - 1,
            UnityEngine.Random.value * 2 - 1,
            UnityEngine.Random.value * 2 - 1);

		shake = shakeCollisionConversion * 6;
	}

    void DockedCam(PlayerShip player, SpaceStation station)
    {
        transform.rotation = Quaternion.identity;
        transform.position = station.transform.position;
        transform.position -= new Vector3(0, 0, 100);
    }

    void FlightCam(PlayerShip player)
    {
        transform.position = offset;

        if (!player)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        if (!ignoreTranslation)
        {
            var newPos = player.transform.TransformPoint(offset);
            newPos -= currentSpeedOffset;

            var shakePhase = shakeSpeed * Time.frameCount;

            var shakeAmount = new Vector3(
                shake * Mathf.Sin(shakePhase * shakeAngles.x),
                shake * Mathf.Sin(shakePhase * shakeAngles.y),
                shake * Mathf.Sin(shakePhase * shakeAngles.z));
            newPos += transform.TransformDirection(shakeAmount);

            transform.position = newPos;

            transform.LookAt(player.transform.position + (player.transform.forward * 1000), player.transform.up);
        }
        else
        {
            transform.position = Vector3.zero;
            transform.LookAt(player.transform.forward * 1000, player.transform.up);
        }

        float rotationOffsetAmt = rotationOffset * Mathf.Rad2Deg;
        var rotOffsetAmt = Quaternion.Euler(player.GetComponent<Rigidbody>().angularVelocity.x * rotationOffsetAmt,
            player.GetComponent<Rigidbody>().angularVelocity.y * rotationOffsetAmt,
            player.GetComponent<Rigidbody>().angularVelocity.z * rotationOffsetAmt);

        var mouselookOffset = Quaternion.Euler(lookPitch * 180, lookYaw * 180, 0);

        transform.rotation =  rotOffsetAmt * transform.rotation * mouselookOffset;	
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

    private Vector2? GetDragTurnAmount(Vector3 mouseRayOrigin)
    {
        var screenAim = GetScreenAimPoint(mouseRayOrigin);

        if (screenAim.HasValue)
        {
            var x = Mathf.Clamp((2 * (screenAim.Value.x / Screen.width)) - 1, -1, 1);
            var y = -Mathf.Clamp((2 * (screenAim.Value.y / Screen.height)) - 1, -1, 1);

            return new Vector2(x, y);
        }
        else
        {
            return Vector2.zero;
        }
    }

    public Vector3? GetWorldAimPoint(Vector3 origin)
    {
        var cursorPos = GetAimCursorPos();
        if (!cursorPos.HasValue)
        {
            return null;
        }

        const float AIM_DEPTH = 1000;
        
        Vector3 mousePos = new Vector3(cursorPos.Value.x, cursorPos.Value.y, AIM_DEPTH);

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
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            dragInput = GetDragTurnAmount(player.transform.position);
        }
        else
        {
            dragInput = null;
        }

        if (Input.GetButton("look") && dragInput != null)
        {
            lookPitch = DragInput.Value.y;
            lookYaw = DragInput.Value.x;
        }
        else
        {
            lookPitch = 0;
            lookYaw = 0;
        }
    }

    private IEnumerator WaitToDragOnGui()
    {
        yield return new WaitForSeconds(0.2f);

        UpdateDrag();
        waitToDragOnGui = null;
    }

    private void Update()
    {
        if ((Input.GetButtonUp("turn") || Input.GetButtonUp("look")) 
            && waitToDragOnGui != null)
        {
            StopCoroutine(waitToDragOnGui);
        }

        if ((Input.GetButtonDown("look") || Input.GetButtonDown("turn"))
            && waitToDragOnGui == null
            && EventSystem.current.IsPointerOverGameObject())
        {
            waitToDragOnGui = StartCoroutine(WaitToDragOnGui());
        }

        var lookButton = Input.GetButton("look");
        var turnButton = Input.GetButton("turn");
        if (turnButton || lookButton)
        {
            if (waitToDragOnGui == null)
            {
                UpdateDrag();
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
		if (!player)
		{
			return;
		}

		shake = Mathf.Lerp(shake, 0, 0.5f);

		var playerRb = player.GetComponent<Rigidbody>();
		var ship = player.GetComponent<Ship>();
		
		if (playerRb && ship)
		{
			var speed = playerRb.velocity / Mathf.Max(1, ship.BaseStats.maxSpeed);
			var targetSpeedOffset = speed * thrustOffset;

			currentSpeedOffset = Vector3.Lerp(currentSpeedOffset, targetSpeedOffset, 0.1f);
		}
	}
}
