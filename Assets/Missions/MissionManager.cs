using UnityEngine;
using System;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    
    [SerializeField]
    private ActiveMission mission;

    [HideInInspector]
    [SerializeField]
    private MissionPhase phase; 
    
    public ActiveMission Mission { get { return mission; } }
    public MissionPhase Phase { get { return phase; } }

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }
        
    private void Update()
    {
        if (phase == MissionPhase.Active)
        {
            var winners = new string[mission.Definition.Teams.Length];
            int winnerCount = 0;

            foreach (var team in mission.Definition.Teams)
            {
                var objectives = ActiveMission.FindObjectives(team.Name);

                bool allComplete = true;
                foreach (var objective in objectives)
                {
                    if (!objective.Complete)
                    {
                        allComplete = false;
                        break;
                    }
                }

                if (allComplete)
                {
                    winners[winnerCount] = team.Name;
                    winnerCount++;
                }
            }

            if (winnerCount > 0)
            {
                Array.Resize(ref winners, winnerCount);
                EndMission(winners);
            }
        }
    }

    private void Awake()
    {
        phase = MissionPhase.Prep;

        if (mission.Definition != null)
        {
            mission.Init();
        }
    }
    
    public void BeginMission()
    {
        phase = MissionPhase.Active;

        foreach (var listener in GameObject.FindGameObjectsWithTag("MissionListener"))
        {
            listener.SendMessage("OnBeginMission", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void EndMission(string[] winningTeams)
    {
        phase = MissionPhase.Finished;
        mission.WinningTeams = winningTeams;

        foreach (var listener in GameObject.FindGameObjectsWithTag("MissionListener"))
        {
            listener.SendMessage("OnEndMission", SendMessageOptions.DontRequireReceiver);
        }
    }
}
