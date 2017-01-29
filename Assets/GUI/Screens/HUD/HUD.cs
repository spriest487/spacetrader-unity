#pragma warning disable 0649

using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [Header("Player Info")]

    [SerializeField]
    private Image playerPortrait;

    [SerializeField]
    private Text playerMoney;

    [Header("Overlays")]

    private LootWindow lootWindow;

    [SerializeField]
    private Button useTargetButton;
    private Text useTargetText;

    private RadioMenu radioMenu;

    [SerializeField]
    private MessagePanel messagePanel;

    [SerializeField]
    private GUIElement cinemaBars;

    [Header("Touch Controls")]

    [SerializeField]
    private List<Canvas> touchControlRoots;

    private void Awake()
    {
        foreach (var touchControl in touchControlRoots)
        {
            touchControl.sortingLayerName = "GUI Overlay";
            touchControl.overrideSorting = true;
        }

        useTargetText = useTargetButton.GetComponentInChildren<Text>(true);

        radioMenu = GetComponentInChildren<RadioMenu>(true);
        radioMenu.GetComponent<GUIElement>().OnTransitionedOut += () =>
            radioMenu.gameObject.SetActive(false);

        lootWindow = GetComponentInChildren<LootWindow>(true);
        lootWindow.GetComponent<GUIElement>().OnTransitionedOut += () =>
            lootWindow.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        lootWindow.gameObject.SetActive(false);
        radioMenu.gameObject.SetActive(false);
        cinemaBars.gameObject.SetActive(false);

        bool showTouchControls = Universe.TouchControlsEnabled
            && !VRSettings.enabled;
        foreach (var touchControl in touchControlRoots)
        {
            touchControl.gameObject.SetActive(showTouchControls);
        }
    }

    private void Update()
    {
        var player = Universe.LocalPlayer;

        if (!player || !player.Ship)
        {
            content.gameObject.SetActive(false);
            cinemaBars.gameObject.SetActive(false);
        }
        else
        {
            if (VRSettings.enabled)
            {
                cinemaBars.gameObject.SetActive(false);
            }
            else
            {
                cinemaBars.Activate(player.Dockable.AutoDockingStation
                    || player.Ship.JumpTarget
                    || GUIController.Current.CutsceneOverlay.HasCutscene);
            }

            content.gameObject.SetActive(!cinemaBars.Activated);

            var playerTarget = player.Ship.Target;

            if (playerTarget
                && playerTarget.ActionOnActivate
                && playerTarget.ActionOnActivate.CanBeActivatedBy(player.Ship)
                && !lootWindow.isActiveAndEnabled)
            {
                useTargetButton.gameObject.SetActive(true);
                useTargetText.text = playerTarget.ActionOnActivate.ActionName;
            }
            else
            {
                useTargetButton.gameObject.SetActive(false);
            }

            if (content.gameObject.activeSelf)
            {
                //TODO: slow
                var captain = player.Ship.GetCaptain();
                if (captain)
                {
                    playerPortrait.sprite = captain.Portrait;
                }
                else
                {
                    playerPortrait.sprite = Universe.CrewConfiguration.DefaultPortrait;
                }

                var money = Market.FormatCurrency(player.Money);
                playerMoney.text = money;

                float messageAlpha = messagePanel.MessageCount > 0? 1 : 0;
                messagePanel.GetComponent<CanvasGroup>().alpha = messageAlpha;
            }

            if (Input.GetButtonDown("radio"))
            {
                radioMenu.Element.Activate(!radioMenu.gameObject.activeSelf);
                if (lootWindow.isActiveAndEnabled)
                {
                    lootWindow.Element.Dismiss();
                }
            }
        }
    }

    //"use target" hud button event
    public void PlayerActivateTarget()
    {
        Universe.LocalPlayer.ActivateTarget();
    }

    //gui screens menu button event
    public void ShowScreensList()
    {
        GUIController.Current.SwitchTo(ScreenID.ScreensList);
    }

    //activated a loot can, pop up the loot display window
    private void OnPlayerActivatedLoot(LootContainer loot)
    {
        if (lootWindow.isActiveAndEnabled && lootWindow.Container == loot)
        {
            lootWindow.TakeAll();
        }
        else
        {
            lootWindow.ShowLoot(loot);
            radioMenu.Element.Dismiss();
        }
    }

    private void OnPlayerError(string message)
    {
        Debug.LogError("TODO proper error logging for: " + message);
        //errorMessage.ShowError(message);
    }
}
