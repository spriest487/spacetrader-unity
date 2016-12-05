#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class MissionPrepScreen : MonoBehaviour
{
    [SerializeField]
    private Text missionDescriptionText;

    [SerializeField]
    private Text readyText;
    
    private GUIScreen guiScreen;
    
    private void Awake()
    {
        guiScreen = GetComponent<GUIScreen>();
    }

    public void Ready()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.BeginMission();
        }

        GUIController.Current.SwitchTo(ScreenID.None);
    }

    private void CancelMissionOnExit()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.CancelMission();
        }
    }
    
    private void OnEnable()
    {
        GetComponent<GUIElement>().OnTransitionedOut += CancelMissionOnExit;

        var mission = MissionManager.Instance.Mission;
        Debug.Assert(mission, "must have an active mission to use the mission prep screen");
        
        missionDescriptionText.text = mission.Definition.Description;
        guiScreen.HeaderText = mission.Definition.MissionName.ToUpper();
    }

    private void OnDisable()
    {
        GetComponent<GUIElement>().OnTransitionedOut -= CancelMissionOnExit;
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
