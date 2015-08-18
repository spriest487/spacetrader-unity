using UnityEngine;

[RequireComponent(typeof(TeamSpawner))]
public class DeathmatchObjective : MonoBehaviour
{
    private const string DESCRIPTION = "Destroy all enemy ships";

    [System.Serializable]
    public class TeamObjective
    {
        public string team;
        public MissionObjective objective;
    }

    private TeamSpawner spawner;
    private TeamObjective[] objectives;

    void Start()
    {
        spawner = GetComponent<TeamSpawner>();
    }

    void OnBeginMission()
    {
        var teamCount = spawner.Teams.Length;
        objectives = new TeamObjective[teamCount];

        for (int teamIndex = 0; teamIndex < teamCount; ++teamIndex)
        {
            var team = spawner.Teams[teamIndex];

            var objectiveObj = new GameObject(team.Name + " deathmatch objective");

            var objective = objectiveObj.AddComponent<MissionObjective>();
            objective.Complete = false;
            objective.Description = DESCRIPTION;
            objective.Teams = new []{ team.Name };
            objective.tag = MissionManager.ActiveMission.MISSION_TAG;

            var teamObjective = new TeamObjective();
            teamObjective.team = team.Name;
            teamObjective.objective = objective;

            objectives[teamIndex] = teamObjective;
        }
    }
    
    void Update()
    {
        if (objectives != null)
        {
            foreach (var teamObjective in objectives)
            {
                int aliveEnemyCount = 0;

                foreach (var team in spawner.Teams)
                {
                    //skip my own team, we're friends!
                    if (team.Name == teamObjective.team)
                    {
                        continue;
                    }

                    foreach (var spawnedShip in team.SpawnedShips)
                    {
                        if (spawnedShip)
                        {
                            aliveEnemyCount++;
                        }
                    }
                }

                if (aliveEnemyCount == 0)
                {
                    teamObjective.objective.Description = DESCRIPTION;
                    teamObjective.objective.Complete = true;
                }
                else
                {
                    teamObjective.objective.Description = DESCRIPTION
                        + string.Format(" ({0} remaining)", aliveEnemyCount);
                    teamObjective.objective.Complete = false;
                }
            }
        }
    }
}
