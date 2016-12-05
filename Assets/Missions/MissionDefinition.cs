#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Missions/Mission Definition")]
public class MissionDefinition : ScriptableObject
{
    [System.Serializable]
    public class PlayerSlot
    {
        [SerializeField]
        private ShipType shipType;

        [SerializeField]
        private ModulePreset modulePreset;

        [SerializeField]
        private string name;

        public ShipType ShipType { get { return shipType; } }
        public ModulePreset ModulePreset { get { return modulePreset; } }
        public string Name { get { return name; } }

        public Ship SpawnShip(Vector3 pos, Quaternion rot, TeamDefinition team)
        {
            var ship = ShipType.CreateShip(pos, rot);
            ship.name = Name;

            var targetable = ship.gameObject.AddComponent<Targetable>();
            targetable.Faction = team.Name;

            if (modulePreset)
            {
                modulePreset.Apply(ship);
            }

            return ship;
        }
    }

    [System.Serializable]
    public class TeamDefinition
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private List<PlayerSlot> slots;

        [SerializeField]
        private List<Quest> quests;

        public string Name { get { return name; } }
        public IList<PlayerSlot> Slots { get { return slots; } }
        public IList<Quest> Quests { get { return quests; } }
    }

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private string missionName;

    [TextArea]
    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite image;

    [SerializeField]
    private List<TeamDefinition> teams;

    public string SceneName { get { return sceneName; } }
    public string MissionName { get { return missionName; } }
    public string Description { get { return description; } }
    public Sprite Image { get { return image; } }

    public IList<TeamDefinition> Teams { get { return teams; } }

    public void LoadMission()
    {
        SceneManager.LoadScene(SceneName);
    }
}
