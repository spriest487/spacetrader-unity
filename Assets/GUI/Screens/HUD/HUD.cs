#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private Transform cinemaBars;
    
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
    private ErrorMessage errorMessage;

    [SerializeField]
    private Button useTargetButton;
    private Text useTargetText;
        
    private void Start()
    {
        useTargetText = useTargetButton.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        var player = PlayerShip.LocalPlayer;

        bool cinemaMode = ScreenManager.Instance.CurrentCutscenePage != null
            || (player && ((player.Moorable && player.Moorable.State == DockingState.AutoDocking)
                || (player.Ship && player.Ship.JumpTarget)));
        
        content.gameObject.SetActive(!cinemaMode);
        cinemaBars.gameObject.SetActive(cinemaMode);

        if (!player || !player.Ship)
        {
            content.gameObject.SetActive(false);
        }
        else if (!cinemaMode)
        {
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
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.ScreensList);
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

    private void OnScreenActive()
    {
        errorMessage.Reset();

        lootWindow.Dismiss();

        Update();
    }

    private void OnPlayerError(string message)
    {
        errorMessage.ShowError(message);
    }
}