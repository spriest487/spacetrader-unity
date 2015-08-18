using UnityEngine;
using System.Collections;

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

    [System.Serializable]
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

    [System.Serializable]
    public class ActiveMission
    {
        public const string MISSION_TAG = "MissionObjective";

        [SerializeField]
        private MissionDefinition missionDefinition;

        [SerializeField]
        private ActivePlayerSlot[] players;

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

    public void EndMission()
    {
        phase = MissionPhase.Finished;

        foreach (var listener in GameObject.FindGameObjectsWithTag("MissionListener"))
        {
            listener.SendMessage("OnEndMission", SendMessageOptions.DontRequireReceiver);
        }
    }
}
