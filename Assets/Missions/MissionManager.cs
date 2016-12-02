#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    
    [SerializeField]
    private ActiveMission mission;

    [HideInInspector]
    [SerializeField]
    private MissionPhase phase;

    private Scene? missionScene;
    
    public ActiveMission Mission { get { return mission; } }
    public MissionPhase Phase { get { return phase; } }
                
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += SceneChanged;

        phase = MissionPhase.Prep;

        var scene = SceneManager.GetActiveScene();
        SceneChanged(scene, scene);
    }

    private void SceneChanged(Scene oldScene, Scene newScene)
    {
        if (SpaceTraderConfig.LocalPlayer)
        {
            return;
        }

        /* if loading into a new scene with no player, and it's a mission scene,
         start that mission */
        foreach (var missionDef in SpaceTraderConfig.MissionsConfiguration.Missions)
        {
            if (missionDef.SceneName == newScene.name)
            {
                Debug.Assert(!missionScene.HasValue, "can't load two missions scenes at once");

                mission = ActiveMission.Create(missionDef);
                missionScene = newScene;
            }
        }
    }

    private void SceneUnloaded(Scene scene)
    {
        if (scene == missionScene)
        {
            missionScene = null;
        }
    }
    
    public void BeginMission()
    {
        phase = MissionPhase.Active;
        
        foreach (var listener in GameObject.FindGameObjectsWithTag("MissionListener"))
        {
            listener.SendMessage("OnBeginMission", SendMessageOptions.DontRequireReceiver);
        }

        //assign quests
        for (int teamIt = 0; teamIt < mission.Teams.Length; ++teamIt)
        {
            var activeTeam = mission.Teams[teamIt];
            var defTeam = mission.Definition.Teams[teamIt];

            for (int slotIt = 0; slotIt < defTeam.Slots.Count; ++slotIt)
            {
                var slot = activeTeam.Slots[slotIt];

                if (slot.Status != SlotStatus.Human)
                {
                    continue;
                }

                var player = slot.SpawnedShip.GetComponent<PlayerShip>();

                foreach (var quest in defTeam.Quests)
                {
                    var playerQuest = Instantiate(quest);
                    SpaceTraderConfig.QuestBoard.NewQuest(playerQuest);
                    SpaceTraderConfig.QuestBoard.AcceptQuest(player, playerQuest);
                }
            }
        }
    }
    
    private IEnumerator CancelMissionRoutine()
    {
        Debug.Assert(phase == MissionPhase.Prep, "can't cancel mission setup if it's already started");
        Debug.Assert(missionScene.HasValue, "must have a mission scene loaded");
        Debug.Assert(SceneManager.GetActiveScene().buildIndex == missionScene.Value.buildIndex, "active scene must be the mission scene");

        mission = null;
        var globalsScene = SceneManager.GetSceneByName("Globals");
        Debug.Assert(globalsScene.isLoaded, "globals scene must be loaded");

        SceneManager.SetActiveScene(globalsScene);
        yield return null;

        yield return SceneManager.UnloadScene(missionScene.Value);
    }

    public Coroutine CancelMission()
    {
        return StartCoroutine(CancelMissionRoutine());
    }

    public void EndMission()
    {
        phase = MissionPhase.Finished;

        foreach (var listener in GameObject.FindGameObjectsWithTag("MissionListener"))
        {
            listener.SendMessage("OnEndMission", SendMessageOptions.DontRequireReceiver);
        }
    }
}
