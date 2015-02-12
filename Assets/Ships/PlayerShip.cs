using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Ship), typeof(ContextInjector))]
[ContextSingleton]
public class PlayerShip : MonoBehaviour
{
	private Ship ship;
    private Moorable moorable;

    [Injected]
    internal ContextComponent<ScreenManager> screenManager = null;

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
    
	private void RotateTowardsAim(Vector2 screenAimPos)
	{
		var aimPitch = -Mathf.Clamp((2 * (screenAimPos.y / Screen.height)) - 1, -1, 1);
		var aimYaw = Mathf.Clamp((2 * (screenAimPos.x / Screen.width)) - 1, -1, 1);

		ship.yaw = aimYaw;
		ship.pitch = aimPitch;
	}
    
    void Update()
    {
        var touchPos = FindTouchPos();
        if (touchPos.HasValue)
        {
            RotateTowardsAim(touchPos.Value);
        }
        else if (ship)
        {
            ship.pitch = Input.GetAxis("pitch");
            ship.yaw = Input.GetAxis("yaw");
        }

        //roll is manual only
        ship.roll = -Input.GetAxis("roll");

        ship.thrust = Input.GetAxis("Vertical");
        ship.strafe = Input.GetAxis("Horizontal");
        ship.lift = Input.GetAxis("lift");

        var loadout = GetComponent<ModuleLoadout>();
        if (loadout)
        {
            if (Input.GetButton("fire"))
            {
                if (loadout.FrontModules.Size > 0)
                {
                    loadout.Activate(0);
                }
            }
        }

        if (Input.GetButtonDown("activate"))
        {
            if (moorable && moorable.spaceStation)
            {
                moorable.RequestMooring();
            }
        }
    }

    void OnMoored()
    {
        if (screenManager.HasValue)
        {
            screenManager.Value.ingameState = ScreenManager.IngameState.Docked;
        }
    }

    void OnUnmoored()
    {
        if (screenManager.HasValue)
        {
            screenManager.Value.ingameState = ScreenManager.IngameState.Flight;
        }
    }
	
	void FixedUpdate ()
	{
        var touchPos = FindTouchPos();

		Vector2? mouseAimPos = null;

		if (touchPos.HasValue)
		{
			mouseAimPos = touchPos;
		}
		else if (Input.mousePresent)
		{
			mouseAimPos = Input.mousePosition;
		}

		if (mouseAimPos.HasValue && Camera.main)
		{
			Vector3 mousePos = new Vector3(mouseAimPos.Value.x, mouseAimPos.Value.y, 1000);
			var mouseRay = Camera.main.ScreenPointToRay(mousePos);

			RaycastHit rayHit;

			if (Physics.Raycast(mouseRay, out rayHit))
			{
				ship.aim = rayHit.point;
			}
			else
			{
				ship.aim = mouseRay.origin + mouseRay.direction * 1000;
			}
		}
		else
		{
			ship.aim = transform.position + transform.forward * 1000;
		}
	}

	void Start()
	{
		ship = GetComponent<Ship>();
        moorable = GetComponent<Moorable>();
	}

	void OnCollisionEnter(Collision collision)
	{
		var followCam = Camera.main ? Camera.main.GetComponent<FollowCamera>() : null;
		if (followCam)
		{
			followCam.NotifyPlayerCollision(collision);
		}
	}
}
