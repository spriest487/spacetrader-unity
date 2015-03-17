﻿using UnityEngine;
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
    }


    [SerializeField]
    private static MissionManager instance;

    [SerializeField]
    private ActiveMission mission;

    public static MissionManager Instance { get { return instance; } }

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
        instance = this;
    }

    void OnDisable()
    {
        instance = null;
    }

    void Start()
    {
        phase = MissionPhase.Prep;

        ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.MissionPrep, ScreenManager.ScreenState.None);
    }

    void Update()
    {
        if (mission.)
    }

    void BeginMission()
    {
        phase = MissionPhase.Active;
    }

    void EndMission()
    {
        phase = MissionPhase.Finished;
    }
}
