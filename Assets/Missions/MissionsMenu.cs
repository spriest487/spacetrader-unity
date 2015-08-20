using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    private ConnectingScreen connectingScreen;

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

        foreach (var mission in MissionManager.Instance.Missions)
        {
            var missionItem = Instantiate(missionElementPrefab);
            missionItem.transform.SetParent(missionsLayout, false);
            missionItem.MissionDefinition = mission;
        }
    }

    public void PlayOffline()
    {
        Application.LoadLevel(selectedMission.SceneName);
    }

    public void HostGame()
    {
        var error = Network.InitializeServer(8, 30001, true);
        if (error == NetworkConnectionError.NoError)
        {
            Application.LoadLevel(selectedMission.SceneName);
            SelectMission(null);
        }
        else
        {
            Debug.Log(error.ToString());
        }
    }

    public void JoinGame()
    {
        var error = Network.Connect("localhost", 30001);
        if (error == NetworkConnectionError.NoError)
        {
            Application.LoadLevel(emptySceneName);
            
            var connecting = (ConnectingScreen) Instantiate(connectingScreen);
            DontDestroyOnLoad(connecting);

            connecting.scene = selectedMission.SceneName;

            SelectMission(null);
        }
        else
        {
            Debug.Log(error.ToString());
        }
    }
    
    void OnFailedToConnect(NetworkConnectionError e)
    {
        Debug.Log("failed to connect: " +e);
    }

    void OnDisconnectedFromServer()
    {
        Debug.Log("disconnected");

        var mainMenu = GetComponentInParent<MainMenu>();
        mainMenu.EndGame();
    }
}