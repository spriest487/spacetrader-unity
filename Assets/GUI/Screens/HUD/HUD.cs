#pragma warning disable 0649

using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [Header("Player Info")]

    [SerializeField]
    private Image playerPortrait;

    [SerializeField]
    private Text playerName;

    [SerializeField]
    private Text playerMoney;

    [Header("Overlays")]

    [SerializeField]
    private LootWindow lootWindow;

    [SerializeField]
    private Button useTargetButton;
    private Text useTargetText;

    private RadioMenu radioMenu;

    private Animator animator;

    private static readonly int CutsceneParamName = Animator.StringToHash("Cutscene");

    private bool CutsceneParam
    {
        get { return animator.GetBool(CutsceneParamName); }
        set { animator.SetBool(CutsceneParamName, value); }
    }

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        useTargetText = useTargetButton.GetComponentInChildren<Text>();

        radioMenu = GetComponentInChildren<RadioMenu>(true);

        lootWindow.Dismiss();
        radioMenu.Dismiss();
    }

    private void Update()
    {
        var player = PlayerShip.LocalPlayer;

        if (!player || !player.Ship)
        {
            content.gameObject.SetActive(false);
            CutsceneParam = false;
        }
        else
        {
            if (VRSettings.enabled)
            {
                CutsceneParam = false;
            }
            else
            {
                CutsceneParam = player.Ship.Moorable.AutoDockingStation
                    || player.Ship.JumpTarget
                    || GUIController.Current.CutsceneOverlay.HasCutscene;
            }

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

            //TODO: slow
            var captain = player.Ship.GetCaptain();
            if (captain)
            {
                playerName.text = captain.name;
                playerPortrait.sprite = captain.Portrait;
            }
            else
            {
                playerName.text = "Player";
                playerPortrait.sprite = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
            }

            var money = Market.FormatCurrency(player.Money);
            playerMoney.text = money;
        }
    }

    //"use target" hud button event
    public void PlayerActivateTarget()
    {
        PlayerShip.LocalPlayer.ActivateTarget();
    }

    //gui screens menu button event
    public void ShowScreensList()
    {
        GUIController.Current.SwitchTo(ScreenID.ScreensList);
    }

    //activated a loot can, pop up the loot display window
    private void OnPlayerActivatedLoot(LootContainer loot)
    {
        if (lootWindow.Container == loot)
        {
            lootWindow.TakeAll();
        }
        else
        {
            lootWindow.ShowLoot(loot);
        }
    }

    private void OnPlayerError(string message)
    {
        Debug.LogError("TODO proper error logging for: " + message);
        //errorMessage.ShowError(message);
    }
}