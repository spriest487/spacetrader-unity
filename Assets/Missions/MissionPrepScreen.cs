using UnityEngine;
using UnityEngine.UI;

public class MissionPrepScreen : MonoBehaviour
{
    [SerializeField]
    private Text missionDescriptionText;

    [SerializeField]
    private Text missionTitleText;

    private Button readyButton;
    private Text readyText;
    
    public void Ready()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.BeginMission();
        }

        ScreenManager.Instance.SetStates(
            hudOverlay: ScreenManager.HudOverlayState.None, 
            state: ScreenManager.ScreenState.Flight);
    }

    void Start()
    {
        readyButton = transform.Find("Ready Button").GetComponent<Button>();
        readyText = readyButton.GetComponentInChildren<Text>();
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
