﻿#pragma warning disable 0649

using UnityEngine;
using System.Linq;
using System.Collections;

struct PlayerRadioMessage
{
    public string Message;
    public Ship Source;
}

[RequireComponent(typeof(Ship), typeof(Moorable))]
public class PlayerShip : MonoBehaviour
{
    [System.Obsolete]
    public static PlayerShip LocalPlayer
    {
        get
        {
            return SpaceTraderConfig.LocalPlayer;
        }
    }

    private Ship ship;

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

    public static PlayerShip MakePlayer(Ship nonPlayerShip)
    {
        var player = nonPlayerShip.gameObject.AddComponent<PlayerShip>();
        player.ship = nonPlayerShip;
        return player;
    }

    void OnMoored()
    {
        if (SpaceTraderConfig.LocalPlayer == this)
        {
            GUIController.Current.SwitchTo(ScreenID.ScreensList);
        }
    }

    void OnUnmoored()
    {
        if (SpaceTraderConfig.LocalPlayer == this)
        {
            GUIController.Current.SwitchTo(ScreenID.HUD);
        }
    }

    bool LocalPlayerHasControl()
    {
        return SpaceTraderConfig.LocalPlayer == this
            && Moorable.State == DockingState.InSpace
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
        if (number < ship.Abilities.Count())
        {
            ship.GetAbility(number).Use(ship);
        }
    }

    void Update()
    {
        if (LocalPlayerHasControl())
        {
            ProcessLocalInput();
        }
    }

    private void ProcessLocalInput()
    {
        if (GUIController.Current.CutsceneOverlay.CurrentCutscenePage != null)
        {
            if (Input.GetButtonDown("fire") || Input.GetButtonDown("activate"))
            {
                GUIController.Current.CutsceneOverlay.AdvanceCutscene();
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
            var thrust = Input.GetAxis("Vertical");

            if (SpaceTraderConfig.TouchControlsEnabled)
            {
                pitch += -TouchJoystick.Value.y;
                yaw += TouchJoystick.Value.x;
                thrust += TouchThrottle.Value ?? 0;
            }
            else if (FollowCamera.Current)
            {
                UpdateModuleAimPoints(FollowCamera.Current);

                if (Input.GetButton("turn"))
                {
                    var turnAim = FollowCamera.Current.DragInput;

                    if (turnAim.HasValue)
                    {
                        yaw += turnAim.Value.x;
                        pitch += turnAim.Value.y;
                    }
                }
            }

            ship.ResetControls(
                pitch: pitch,
                yaw: yaw,
                roll: -Input.GetAxis("roll"),
                thrust: thrust,
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

            if (Input.GetButtonDown("activate"))
            {
                ActivateTarget();
            }
        }
    }

    void OnRadioMessage(RadioMessage message)
    {
        switch (message.MessageType)
        {
            case RadioMessageType.Greeting:
                // var recipient = (Ship.Target && Ship.Target == message.SourceShip.Targetable) ?
                //     Ship.Target.name : name;
                break;

            case RadioMessageType.AcknowledgeOrder:
                if (message.SourceShip != this)
                {
                    string msg = "OK, Boss.";

                    GUIController.Current.BroadcastMessage("OnPlayerNotification",
                        message.SourceShip.name + "> " + msg,
                        SendMessageOptions.DontRequireReceiver);

                    GUIController.Current.BroadcastMessage("OnRadioSpeech",
                        new PlayerRadioMessage()
                        {
                            Message = msg,
                            Source = message.SourceShip
                        },
                        SendMessageOptions.DontRequireReceiver);
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
        yield return GUIController.Current.ShowLoadingOverlay();

        yield return SpaceTraderConfig.WorldMap.LoadArea(area);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        GUIController.Current.DismissLoadingOverlay();
        yield return GUIController.Current.SwitchTo(ScreenID.None);
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

        GUIController.Current.BroadcastMessage("OnPlayerNotification", msg, SendMessageOptions.DontRequireReceiver);
    }

	void Awake()
	{
		ship = GetComponent<Ship>();
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
