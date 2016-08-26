#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class HudObjectivesList : MonoBehaviour
{
    [SerializeField]
    private Text header;

    [SerializeField]
    private HudObjectivesItem objectiveItemPrefab;

    [SerializeField]
    private Transform objectivesRoot;

    private PooledList<HudObjectivesItem, MissionObjective> objectivesItems;

    void Update()
    {
        var player = PlayerShip.LocalPlayer;

        if (objectivesItems == null)
        {
            objectivesItems = new PooledList<HudObjectivesItem, MissionObjective>(objectivesRoot, objectiveItemPrefab);
        }

        var faction = player.Ship.Targetable.Faction;

        var objectives = ActiveMission.FindObjectives(faction);

        objectivesItems.Refresh(objectives, (i, item, objective) =>
        {
            item.Assign(objective);
        });

        header.gameObject.SetActive(objectives.Length > 0);
    }
}