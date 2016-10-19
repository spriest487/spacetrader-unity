#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class MissionPrepScreen : MonoBehaviour
{
    [SerializeField]
    private Text missionDescriptionText;

    [SerializeField]
    private Text missionTitleText;
    
    //[SerializeField]
    //private Button readyButton;

    [SerializeField]
    private Text readyText;
    
    public void Ready()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.BeginMission();
        }

        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
    
    void OnScreenActive()
    {
        var mission = MissionManager.Instance.Mission;

        if (mission != null)
        {
            missionDescriptionText.text = mission.Definition.Description;
            missionTitleText.text = mission.Definition.MissionName.ToUpper();
        }
    }
    
    void Update()
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
