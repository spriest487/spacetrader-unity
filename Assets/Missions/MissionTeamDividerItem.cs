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
        var mission = MissionManager.Instance.Mission;

        if (teamIndex >= 0 && mission)
        {
            var team = mission.Definition.GetTeam(teamIndex);

            teamNameLabel.text = team.Name.ToUpper();
        }
    }
}