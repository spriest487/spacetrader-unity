using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TeamSpawner))]
public class TeamDeathmatchVictory : MonoBehaviour
{
    private TeamSpawner teamSpawner;

    void Start()
    {
        teamSpawner = GetComponent<TeamSpawner>();
    }
    
    void Update()
    {        
        if (MissionManager.Instance.Phase == MissionManager.MissionPhase.Active)
        {
            int remainingTeamCount = 0;
            TeamSpawner.Team remainingTeam = null;

            for (int team = 0; team < teamSpawner.Teams.Length; ++team)
            {
                foreach (var ship in teamSpawner.Teams[team].SpawnedShips)
                {
                    if (ship)
                    {
                        remainingTeamCount++;
                        remainingTeam = teamSpawner.Teams[team];
                        break;
                    }
                }
            }

            if (remainingTeamCount == 1)
            {
                Debug.Log("team " +remainingTeam.Name +" won");
                MissionManager.Instance.EndMission();
            }
            else if (remainingTeamCount == 0)
            {
                Debug.Log("it's a draw");
                MissionManager.Instance.EndMission();
            }
        }
    }
}
