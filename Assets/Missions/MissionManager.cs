#pragma warning disable 0649

using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    
    [SerializeField]
    private ActiveMission mission;

    [HideInInspector]
    [SerializeField]
    private MissionPhase phase; 
    
    public ActiveMission Mission { get { return mission; } }
    public MissionPhase Phase { get { return phase; } }

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }
        
    private void Awake()
    {
        phase = MissionPhase.Prep;
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

    public void EndMission()
    {
        phase = MissionPhase.Finished;

        foreach (var listener in GameObject.FindGameObjectsWithTag("MissionListener"))
        {
            listener.SendMessage("OnEndMission", SendMessageOptions.DontRequireReceiver);
        }
    }
}
