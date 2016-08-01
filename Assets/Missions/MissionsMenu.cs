#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MissionsMenu : MonoBehaviour
{
    [SerializeField]
    private Transform missionsLayout;

    [SerializeField]
    private MissionMenuItem missionElementPrefab;

    [SerializeField]
    private Text selectedMissionTitle;

    [SerializeField]
    private Text selectedMissionDescription;

    [SerializeField]
    private Button[] missionActionButtons;
    
    [SerializeField]
    private string emptySceneName;
    
    private MissionDefinition selectedMission;

    public void SelectMission(MissionDefinition mission)
    {
        selectedMission = mission;

        if (!selectedMission)
        {
            selectedMissionTitle.text = "No mission selected";
            selectedMissionDescription.text = "";
        }
        else
        {
            selectedMissionTitle.text = selectedMission.MissionName;
            selectedMissionDescription.text = selectedMission.Description;
        }

        foreach (var button in missionActionButtons)
        {
            button.interactable = !!selectedMission;
        }
    }

    void OnMenuScreenActivate()
    {
        SelectMission(null);
    }

    void OnMenuScreenDeactivate()
    {
        SelectMission(null);
    }

    void Start()
    {
        if (!missionsLayout
            || !missionElementPrefab
            || !selectedMissionDescription
            || !selectedMissionTitle)
        {
            throw new UnityException("invalid configuration for missions menu");
        }

        foreach (var mission in SpaceTraderConfig.MissionsConfiguration.Missions)
        {
            var missionItem = Instantiate(missionElementPrefab);
            missionItem.transform.SetParent(missionsLayout, false);
            missionItem.MissionDefinition = mission;
        }
    }

    public void PlayOffline()
    {
        SceneManager.LoadScene(selectedMission.SceneName);
    }
}


