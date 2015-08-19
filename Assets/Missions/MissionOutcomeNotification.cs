using UnityEngine;
using UnityEngine.UI;

public class MissionOutcomeNotification : MonoBehaviour
{
    [SerializeField]
    private Text label;

    void OnEndMission()
    {
        var winners = MissionManager.Instance.Mission.WinningTeams;
        var player = PlayerShip.LocalPlayer;

        label.text = "";

        if (player)
        {
            var playerFaction = player.GetComponent<Targetable>().Faction;
            bool playerWon = false;

            foreach (var winner in winners)
            {
                if (winner == playerFaction)
                {
                    playerWon = true;
                    break;
                }
            }
            
            label.text = playerFaction + (playerWon? " won!" : " lost");
            label.text = label.text.ToUpper();
        }
        
        label.gameObject.SetActive(label.text.Length != 0);
    }
}