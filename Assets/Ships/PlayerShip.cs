using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Ship))]
public class PlayerShip : MonoBehaviour
{
    private float? targetingClickStart = null;
    private const float STICKY_TARGET_CLICK_DELAY = 0.1f;

    private static PlayerShip[] localPlayer;

    public static PlayerShip LocalPlayer
    {
        get
        {
            if (localPlayer == null)
            {
                localPlayer = new PlayerShip[] { FindObjectOfType<PlayerShip>() };
            }

            return localPlayer[0];
        }
        private set
        {
            localPlayer = new PlayerShip[] { value };
        }
    }

    private Ship ship;
    private Moorable moorable;

    private bool inputDragging = false;

    [SerializeField]
    private int money;

    public Ship Ship
    {
        get { return ship; }
    }

    public int Money
    {
        get { return money; }
    }

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

    public void AddMoney(int amount)
    {
        money += amount;
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

		ship.Yaw = aimYaw;
		ship.Pitch = aimPitch;
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
#if UNITY_WEBGL
        return true;
#else
        if (Network.isClient || Network.isServer)
        {
            return GetComponent<NetworkView>() && GetComponent<NetworkView>().isMine;
        }
        else {
            return true;
        }
#endif
    }

    private bool AutoaimSnapToPredictor(Vector3 mousePos, ModuleStatus module, WeaponHardpoint hardpoint)
    {
        if (!ship.Target)
        {
            return false;
        }

        /* mouse/touch auto-aim implementation
           calculate predictor pos for this module and convert it to screen - if it's
           within the snap distance, point this module directly at the predictor instead */
        const float AUTOAIM_SNAP_DIST = 30;
        const float AUTOAIM_SNAP_DIST_SQR = AUTOAIM_SNAP_DIST * AUTOAIM_SNAP_DIST;

        var behavior = module.Definition.Behaviour;
        var predictedPos = behavior.PredictTarget(ship, hardpoint, ship.Target);
        if (predictedPos.HasValue)
        {
            var screenPredicted = Camera.main.WorldToScreenPoint(predictedPos.Value);
            screenPredicted.z = mousePos.z;

            var predictedToActualDifference = screenPredicted - mousePos;

            if (predictedToActualDifference.sqrMagnitude < AUTOAIM_SNAP_DIST_SQR)
            {
                module.Aim = predictedPos.Value;
                return true;
            }
        }

        return false;
    }

    private void CalculateMouseAim(Vector3? aimPoint)
    {
        var loadout = GetComponent<ModuleLoadout>();
        if (!loadout)
        {
            return;
        }

        for (int moduleIndex = 0; moduleIndex < loadout.FrontModules.Size; ++moduleIndex)
        {
            var module = loadout.FrontModules[moduleIndex];
            var hardpoint = loadout.FindHardpoint(moduleIndex);

            if (aimPoint.HasValue && Camera.main)
            {
                if (!AutoaimSnapToPredictor(aimPoint.Value, module, hardpoint))
                {
                    Vector3 mouseAim;

                    Vector3 mousePos = new Vector3(aimPoint.Value.x, aimPoint.Value.y, 1000);

                    var mouseRay = Camera.main.ScreenPointToRay(mousePos);

                    Vector3? hitSomething = null;
                    RaycastHit[] rayHits = Physics.RaycastAll(mouseRay);

                    foreach (var rayHit in rayHits)
                    {
                        if (rayHit.collider != GetComponent<Collider>())
                        {
                            hitSomething = rayHit.point;
                            break;
                        }
                    }

                    if (!hitSomething.HasValue)
                    {
                        mouseAim = mouseRay.origin + mouseRay.direction * 1000;
                    }
                    else
                    {
                        mouseAim = hitSomething.Value;
                    }

                    module.Aim = mouseAim;
                }
            }
            else
            {
                module.Aim = ship.transform.position + ship.transform.forward * 1000;
            }
        }
    }

    private void UseAbility(int number)
    {
        if (number < ship.Abilities.Count)
        {
            ship.Abilities[number].Use(ship);
        }
    }

    void Update()
    {
        if (ScreenManager.Instance.CurrentCutscenePage != null)
        {
            if (Input.GetButtonDown("fire") || Input.GetButtonDown("activate"))
            {
                ScreenManager.Instance.AdvanceCutscene();
            }
        }
        else if (HasControl())
        {
            if (Input.GetButtonDown("Use Ability 1"))
            {
                UseAbility(0);
            }

            if (Input.GetButtonDown("Use Ability 2"))
            {
                UseAbility(1);
            }

            if (Input.GetButtonDown("Use Ability 3"))
            {
                UseAbility(2);
            }

            if (Input.GetButtonDown("Use Ability 4"))
            {
                UseAbility(3);
            }

            var aimPoint = FindAimPoint();

            UpdateDrag(aimPoint);
            CalculateMouseAim(aimPoint);

            if (inputDragging && aimPoint.HasValue)
            {
                RotateTowardsAim(aimPoint.Value);
            }
            else
            {
                ship.Pitch = Input.GetAxis("pitch");
                ship.Yaw = Input.GetAxis("yaw");
            }

            if (Input.GetMouseButtonDown(0))
            {
                targetingClickStart = Time.time;
            }

            if (Input.GetMouseButtonUp(0)
                && !EventSystem.current.IsPointerOverGameObject()
                && (!targetingClickStart.HasValue
                    || targetingClickStart.Value + STICKY_TARGET_CLICK_DELAY > Time.time))
            {
                ship.Target = null;
                targetingClickStart = null;
            }

            //roll is manual only
            ship.Roll = -Input.GetAxis("roll");

            ship.Thrust = Input.GetAxis("Vertical");
            ship.Strafe = Input.GetAxis("Horizontal");
            ship.Lift = Input.GetAxis("lift");

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
