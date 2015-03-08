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
    }


    [SerializeField]
    private static MissionManager instance;   

    public static MissionManager Instance { get {return instance; } }
    
    [HideInInspector]
    [SerializeField]
    private bool missionStarted;

    void OnWorldEnd()
    {
        Destroy(gameObject);
    }

    void OnEnable()
    {
        instance = this;
    }

    void OnDisable()
    {
        instance = null;
    }

    void Start()
    {
        missionStarted = false;

        ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.MissionPrep, ScreenManager.ScreenState.None);
    }
}
