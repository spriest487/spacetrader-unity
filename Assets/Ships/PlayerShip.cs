#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

struct PlayerRadioMessage
{
    public string Message;
    public Ship Source;
}

[RequireComponent(typeof(Ship), typeof(Moorable))]
public class PlayerShip : MonoBehaviour
{
    public static PlayerShip LocalPlayer
    {
        get
        {
            return SpaceTraderConfig.LocalPlayer;
        }
    }

    private Ship ship;
    private Moorable moorable;

    [SerializeField]
    private int money;

    public Ship Ship
    {
        get { return ship; }
    }

    public Moorable Moorable
    {
        get { return Ship? Ship.Moorable : null; }
    }
    
    public int Money
    {
        get { return money; }
    }
    
    public void AddMoney(int amount)
    {
        money += amount;
    }
    
    void OnMoored()
    {
        if (LocalPlayer == this)
        {
            ScreenManager.Instance.SetStates(ScreenID.None, PlayerStatus.Docked);
        }
    }

    void OnUnmoored()
    {
        if (LocalPlayer == this)
        {
            ScreenManager.Instance.SetStates(ScreenID.None, PlayerStatus.Flight);
        }
    }

    bool LocalPlayerHasControl()
    {
        return LocalPlayer == this
            && moorable.State == DockingState.InSpace
            && !Ship.JumpTarget;
    }

    private Vector3 AutoaimSnapToPredictor(Vector3 mousePos, int slot)
    {
        if (!ship.Target)
        {
            return mousePos;
        }

        /* mouse/touch auto-aim implementation
           calculate predictor pos for this module and convert it to screen - if it's
           within the snap distance, point this module directly at the predictor instead */
        const float AUTOAIM_SNAP_DIST = 30;
        const float AUTOAIM_SNAP_DIST_SQR = AUTOAIM_SNAP_DIST * AUTOAIM_SNAP_DIST;

        var module = ship.ModuleLoadout.GetSlot(slot);
        if (module.ModuleType)
        {
            var behavior = module.ModuleType.Behaviour;

            var predictedPos = behavior.PredictTarget(ship, slot, ship.Target);
            if (predictedPos.HasValue)
            {
                var screenPredicted = FollowCamera.Current.Camera.WorldToScreenPoint(predictedPos.Value);
                screenPredicted.z = mousePos.z;

                var predictedToActualDifference = screenPredicted - mousePos;

                if (predictedToActualDifference.sqrMagnitude < AUTOAIM_SNAP_DIST_SQR)
                {
                    return predictedPos.Value;
                }
            }
        }

        return mousePos;
    }

    private void UpdateModuleAimPoints(FollowCamera cam)
    {
        var loadout = ship.ModuleLoadout;

        for (int moduleIndex = 0; moduleIndex < loadout.SlotCount; ++moduleIndex)
        {
            if (loadout.IsFreeSlot(moduleIndex))
            {
                continue;
            }

            var module = loadout.GetSlot(moduleIndex);

            var hardpoint = ship.GetHardpointAt(moduleIndex);

            var aimOrigin = hardpoint.transform.position;

            var aimPoint = cam.GetWorldAimPoint(aimOrigin);
            
            if (aimPoint.HasValue)
            {
                module.Aim = AutoaimSnapToPredictor(aimPoint.Value, moduleIndex);
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
        if (LocalPlayerHasControl() 
            && ScreenManager.Instance.ScreenID == ScreenID.None 
            && ScreenManager.Instance.State == PlayerStatus.Flight)
        {
            ProcessLocalInput();
        }
    }

    private void ProcessLocalInput()
    {
        if (ScreenManager.Instance.CurrentCutscenePage != null)
        {
            if (Input.GetButtonDown("fire") || Input.GetButtonDown("activate"))
            {
                ScreenManager.Instance.AdvanceCutscene();
            }
        }
        else if (LocalPlayerHasControl())
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

            var pitch = Input.GetAxis("pitch");
            var yaw = Input.GetAxis("yaw");

            var camera = FollowCamera.Current;
            if (camera)
            {
                UpdateModuleAimPoints(camera);

                if (Input.GetButton("turn"))
                {
                    var turnAim = camera.DragInput;

                    if (turnAim.HasValue)
                    {
                        yaw = turnAim.Value.x;
                        pitch = turnAim.Value.y;
                    }
                }
            }

            ship.ResetControls(
                pitch: pitch, 
                yaw: yaw, 
                roll: -Input.GetAxis("roll"),
                thrust: Input.GetAxis("Vertical"),
                strafe: Input.GetAxis("Horizontal"),
                lift:  Input.GetAxis("lift"));

            if (Input.GetButton("fire"))
            {
                for (int mod = 0; mod < ship.ModuleLoadout.SlotCount; ++mod)
                {
                    if (!ship.ModuleLoadout.IsFreeSlot(mod))
                    {
                        ship.ModuleLoadout.Activate(ship, mod);
                    }
                }
            }

            if (Input.GetButtonDown("map"))
            {
                ScreenManager.Instance.FadeScreenTransition(ScreenID.WorldMap, 
                    ScreenTransition.FadeToBlack, 
                    ScreenTransition.FadeFromBlack);
            }

            if (Input.GetButtonDown("activate"))
            {
                ActivateTarget();
            }

            if (Input.GetButtonDown("radio"))
            {
                ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "ShowRadioMenu", null);
            }
        }
    }

    void OnRadioMessage(RadioMessage message)
    {
        switch (message.MessageType)
        {
            case RadioMessageType.Greeting:
                if (message.SourceShip == Ship)
                {
                    var target = Ship.Target;

                    if (target)
                    {
                        ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight,
                            ScreenID.None,
                            "OnPlayerNotification",
                            "You> Hello, " + target.name);
                    }
                }    
                else
                {
                    ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight,
                        ScreenID.None,
                        "OnPlayerNotification",
                        message.SourceShip.name + "> Hello!");
                }
                break;

            case RadioMessageType.AcknowledgeOrder:
                if (message.SourceShip != this)
                {
                    string msg = "OK, Boss.";

                    ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight,
                        ScreenID.None,
                        "OnPlayerNotification",
                        message.SourceShip.name + "> " + msg);

                    ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None,
                        "OnRadioSpeech",
                        new PlayerRadioMessage()
                        {
                            Message = msg,
                            Source = message.SourceShip
                        });
                }

                break;
        }
    }

    public void ActivateTarget()
    {
        if (!Ship.Target)
        {
            return;
        }

        if (Ship.Target.ActionOnActivate && Ship.Target.ActionOnActivate.CanBeActivatedBy(Ship))
        {
            Ship.Target.ActionOnActivate.Activate(Ship);
        }
    }

    private IEnumerator LevelTransitionRoutine(WorldMapArea area)
    {
        var loading = ScreenManager.Instance.CreateLoadingScreen();

        yield return SceneManager.LoadSceneAsync(area.name);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        loading.Dismiss();
    }

    private void OnCompletedJump()
    {
        //jumping disables the collider
        Ship.Collider.enabled = true;
        Ship.RigidBody.isKinematic = false;

        StopAllCoroutines();
        StartCoroutine(LevelTransitionRoutine(Ship.JumpTarget));
    }

    private void OnCrewMemberGainedXP(XPGain xpGain)
    {
        var msg = string.Format("{0} gained {1} XP", xpGain.CrewMember.name, xpGain.Amount);

        ScreenManager.Instance.BroadcastPlayerNotification(msg);
    }

	void Start()
	{
		ship = GetComponent<Ship>();
        moorable = GetComponent<Moorable>();

        DontDestroyOnLoad(gameObject);
	}

    void OnDestroy()
    {
        if (SpaceTraderConfig.Instance &&
            SpaceTraderConfig.LocalPlayer == this)
        {
            SpaceTraderConfig.LocalPlayer = null;
        }
    }
}
