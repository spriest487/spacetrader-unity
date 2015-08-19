using UnityEngine;
using System;

public class MissionManager : MonoBehaviour
{
    public enum SlotStatus
    {
        Open,
        Closed,
        Human,
        AI
    }

    public enum MissionPhase
    {
        Prep,
        Active,
        Finished
    }

    [Serializable]
    public class ActivePlayerSlot : MissionDefinition.PlayerSlot
    {
        [SerializeField]
        private SlotStatus slotStatus;

        public SlotStatus Status
        {
            get { return slotStatus; }
            set { slotStatus = value; }
        }
    }

    [Serializable]
    public class ActiveMission
    {
        public const string MISSION_TAG = "MissionObjective";

        [SerializeField]
        private MissionDefinition missionDefinition;

        [SerializeField]
        private ActivePlayerSlot[] players;

        [SerializeField]
        private string[] winningTeams;

        public static MissionObjective[] FindObjectives(string team)
        {
            var allObjectives = GameObject.FindGameObjectsWithTag(MISSION_TAG);

            var teamCount = 0;
            var teamObjectives = new MissionObjective[allObjectives.Length];

            foreach (var obj in allObjectives) {
                var objective = obj.GetComponent<MissionObjective>();

                foreach (var objectiveTeam in objective.Teams)
                {
                    if (objectiveTeam == team)
                    {
                        teamObjectives[teamCount] = objective;
                        teamCount++;
                    }
                }
            }

            var result = new MissionObjective[teamCount];
            for (int objIndex = 0; objIndex < teamCount; ++objIndex)
            {
                result[objIndex] = teamObjectives[objIndex];
            }

            return result;
        }

        public MissionDefinition Definition { get { return missionDefinition; } }
        public ActivePlayerSlot[] Players { get { return players; } }

        public string[] WinningTeams
        {
            get { return winningTeams; }
            set { winningTeams = (string[])value.Clone(); }
        }
    }

    [SerializeField]
    private MissionDefinition[] missions;

    [SerializeField]
    private static MissionManager instance;

    [SerializeField]
    private ActiveMission mission;

    [HideInInspector]
    [SerializeField]
    private MissionPhase phase; 
    
    public static MissionManager Instance { get { return instance; } }

    public MissionDefinition[] Missions { get { return missions; } }
    public ActiveMission Mission { get { return mission; } }
    public MissionPhase Phase { get { return phase; } }

    void OnWorldEnd()
    {
        Destroy(gameObject);
    }

    void OnEnable()
    {
        if (instance && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDisable()
    {
        instance = null;
    }

    void Update()
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

    void Start()
    {
        phase = MissionPhase.Prep;
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
