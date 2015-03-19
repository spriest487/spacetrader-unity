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
        [SerializeField]
        private MissionDefinition missionDefinition;

        [SerializeField]
        private ActivePlayerSlot[] players;

        public bool Completed = false;
    }

    [SerializeField]
    private MissionDefinition[] missions;

    [SerializeField]
    private static MissionManager instance;

    [SerializeField]
    private ActiveMission mission;

    public static MissionManager Instance { get { return instance; } }

    public MissionDefinition[] Missions { get { return missions; } }

    public ActiveMission Mission { get { return mission; } }
    
    [HideInInspector]
    [SerializeField]
    private MissionPhase phase;

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

    void Update()
    {
        if (mission.Completed)
        {
            EndMission();
        }
    }

    public void BeginMission()
    {
        phase = MissionPhase.Active;
    }

    void EndMission()
    {
        phase = MissionPhase.Finished;
    }
}
