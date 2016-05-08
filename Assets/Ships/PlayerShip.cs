﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Ship))]
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

        var module = ship.ModuleLoadout.HardpointModules[slot];
        var behavior = module.ModuleType.Behaviour;

        var predictedPos = behavior.PredictTarget(ship, slot, ship.Target);
        if (predictedPos.HasValue)
        {
            var screenPredicted = Camera.main.WorldToScreenPoint(predictedPos.Value);
            screenPredicted.z = mousePos.z;

            var predictedToActualDifference = screenPredicted - mousePos;

            if (predictedToActualDifference.sqrMagnitude < AUTOAIM_SNAP_DIST_SQR)
            {
                return predictedPos.Value;
            }
        }

        return mousePos;
    }

    private void UpdateModuleAimPoints(FollowCamera cam)
    {
        var loadout = ship.ModuleLoadout;

        for (int moduleIndex = 0; moduleIndex < loadout.HardpointModules.Count; ++moduleIndex)
        {
            var module = loadout.HardpointModules[moduleIndex];
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

            var pitch = Input.GetAxis("pitch");
            var yaw = Input.GetAxis("yaw");

            var camera = Camera.main? Camera.main.GetComponent<FollowCamera>() : null;
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

            ship.Pitch = pitch;
            ship.Yaw = yaw;

            //roll is manual only
            ship.Roll = -Input.GetAxis("roll");

            ship.Thrust = Input.GetAxis("Vertical");
            ship.Strafe = Input.GetAxis("Horizontal");
            ship.Lift = Input.GetAxis("lift");
            
            if (Input.GetButton("fire"))
            {
                if (ship.ModuleLoadout.HardpointModules.Count > 0)
                {
                    for (var mod = 0; mod < ship.ModuleLoadout.HardpointModules.Count; ++mod)
                    {
                        ship.ModuleLoadout.Activate(ship, mod);
                    }
                }
            }
            
            if (Input.GetButtonDown("activate"))
            {
                if (ship.Target)
                {
                    ship.Target.SendMessage("OnActivated", this, SendMessageOptions.DontRequireReceiver);
                }
            }

            if (Input.GetButtonDown("radio"))
            {
                ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "ShowRadioMenu", null);
            }
        }
    }

    void OnRadioMessage(RadioMessage message)
    {
        if (message.MessageType == RadioMessageType.Greeting)
        {
            if (message.Source == Ship)
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
                    message.Source.name + "> Hello!");
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
        if (SpaceTraderConfig.Instance &&
            SpaceTraderConfig.LocalPlayer == this)
        {
            SpaceTraderConfig.LocalPlayer = null;
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
