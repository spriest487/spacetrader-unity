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
        foreach (var mission in SpaceTraderConfig.MissionsConfiguration.Missions)
        {
            var missionItem = MissionMenuItem.Create(missionElementPrefab, mission);
            missionItem.transform.SetParent(missionsLayout, false);
        }
    }

    private IEnumerator PlayOfflineRoutine()
    {
        var gui = GetComponentInParent<GUIController>();
        yield return gui.ShowLoadingOverlay();

        yield return SceneManager.LoadSceneAsync(selectedMission.SceneName);
        
        gui.DismissLoadingOverlay();
        yield return gui.SwitchTo(ScreenID.MissionPrep);
    }

    public void PlayOffline()
    {
        StartCoroutine(PlayOfflineRoutine());
    }
}


