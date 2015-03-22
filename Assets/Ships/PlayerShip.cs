using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Ship))]
public class PlayerShip : MonoBehaviour
{
    public static PlayerShip LocalPlayer { get; private set; }

	private Ship ship;
    private Moorable moorable;

    private bool inputDragging = false;

    public void MakeLocal()
    {
        if (LocalPlayer && LocalPlayer != this)
        {
            throw new UnityException("there is already an active local player");
        }

        LocalPlayer = this;
    }

    public static void ClearLocal()
    {
        LocalPlayer = null;
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

    private Vector2? FindAimPoint()
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

    private void UpdateDrag(Vector2? aimPos)
    {
        //see whether we can start a new drag
        if (aimPos.HasValue && Input.GetMouseButton(0) && !inputDragging)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                inputDragging = true;
            }
        }

        if (!Input.GetMouseButton(0))
        {
            inputDragging = false;
        }
    }
    
	private void RotateTowardsAim(Vector2 screenAimPos)
	{
		var aimPitch = -Mathf.Clamp((2 * (screenAimPos.y / Screen.height)) - 1, -1, 1);
		var aimYaw = Mathf.Clamp((2 * (screenAimPos.x / Screen.width)) - 1, -1, 1);

		ship.yaw = aimYaw;
		ship.pitch = aimPitch;
	}

    private void TargetAimPoint()
    {
        var screenAim = Camera.main.WorldToScreenPoint(ship.aim);
        var ray = Camera.main.ScreenPointToRay(screenAim);

        bool targeted = false;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var targetable = hit.collider.GetComponent<Targetable>();
            if (targetable)
            {
                ship.Target = targetable;
                targeted = true;
            }
        }

        if (!targeted)
        {
            ship.Target = null;
        }
    }

    void OnMoored()
    {
        if (LocalPlayer == this)
        {
            ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.None, ScreenManager.ScreenState.Docked);
        }
    }

    void OnUnmoored()
    {
        if (LocalPlayer == this)
        {
            ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.None, ScreenManager.ScreenState.Flight);
        }
    }

    bool HasControl()
    {
        if (Network.isClient || Network.isServer)
        {
            return GetComponent<NetworkView>() && GetComponent<NetworkView>().isMine;
        }
        else {
            return true;
        }
    }
	
	void FixedUpdate ()
	{
        if (!HasControl())
        {
            return;
        }

        var aimPoint = FindAimPoint();
        UpdateDrag(aimPoint);

        if (aimPoint.HasValue && Camera.main)
		{
            Vector3 mousePos = new Vector3(aimPoint.Value.x, aimPoint.Value.y, 1000);
			var mouseRay = Camera.main.ScreenPointToRay(mousePos);

            bool hitSomething = false;
			RaycastHit[] rayHits = Physics.RaycastAll(mouseRay);
            
            foreach (var rayHit in rayHits)
            {
                if (rayHit.collider != GetComponent<Collider>())
                {
                    ship.aim = rayHit.point;
                    hitSomething = true;
                    break;
                }                    
            }
            if (!hitSomething)
            {
                ship.aim = mouseRay.origin + mouseRay.direction * 1000;
            }
		}
        else
        {
            ship.aim = transform.position + transform.forward * 1000;
        }

        if (inputDragging && aimPoint.HasValue)
        {
            RotateTowardsAim(aimPoint.Value);
        }
        else
        {
            ship.pitch = Input.GetAxis("pitch");
            ship.yaw = Input.GetAxis("yaw");
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ship.Target = null;
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
                    for (var mod = 0; mod < loadout.FrontModules.Size; ++mod)
                    {
                        loadout.Activate(mod);
                    }                        
                }
            }
        }

        if (Input.GetButtonDown("activate"))
        {
            if (moorable && moorable.SpaceStation)
            {
                moorable.RequestMooring();
            }
        }

        if (Input.GetButtonDown("target"))
        {
            TargetAimPoint();
        }
	}

	void Start()
	{
		ship = GetComponent<Ship>();
        moorable = GetComponent<Moorable>();
	}

    void OnDestroy()
    {
        if (LocalPlayer == this)
        {
            ClearLocal();
        }
    }

	void OnCollisionEnter(Collision collision)
	{
        if (!HasControl())
        {
            return;
        }

        var followCam = Camera.main ? Camera.main.GetComponent<FollowCamera>() : null;
        if (followCam)
        {
            followCam.NotifyPlayerCollision(collision);
        }
	}
}
