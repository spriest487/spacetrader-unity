#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System;

public class MissionPrepScreen : MonoBehaviour
{
    [SerializeField]
    private Text missionDescriptionText;

    [SerializeField]
    private Text readyText;

    private GUIScreen guiScreen;
    private GUIController gui;

    private void Awake()
    {
        guiScreen = GetComponent<GUIScreen>();
        gui = GetComponentInParent<GUIController>();
    }

    public void Ready()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.BeginMission();
        }

        GUIController.Current.SwitchTo(ScreenID.None);
    }

    private void CancelMissionOnExit(Action proceed)
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.CancelMission();
        }

        proceed();
    }

    private void OnEnable()
    {
        gui.OnDismiss += CancelMissionOnExit;

        var mission = MissionManager.Instance.Mission;
        Debug.Assert(mission, "must have an active mission to use the mission prep screen");

        missionDescriptionText.text = mission.Definition.Description;
        guiScreen.HeaderText = mission.Definition.MissionName.ToUpper();
    }

    private void OnDisable()
    {
        gui.OnDismiss -= CancelMissionOnExit;
    }

    private void Update()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            readyText.text = "Ready";
        }
        else
        {
            readyText.text = "Close";
        }
    }
}
