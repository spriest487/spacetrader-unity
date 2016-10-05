#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class MissionTeamDividerItem : MonoBehaviour
{
    [SerializeField]
    private Text teamNameLabel;

    [SerializeField]
    private int teamIndex = -1;

    public void SetTeam(int team)
    {
        teamIndex = team;
    }

    void Update()
    {
        if (teamIndex >= 0)
        {
            var mission = MissionManager.Instance.Mission;

            var team = mission.Definition.Teams[teamIndex];

            teamNameLabel.text = team.Name.ToUpper();
        }
    }
}