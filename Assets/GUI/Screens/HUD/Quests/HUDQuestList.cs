#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class HUDQuestList : MonoBehaviour
{
    [SerializeField]
    private Text header;

    [SerializeField]
    private HUDQuestListItem itemPrefab;

    [SerializeField]
    private Transform objectivesRoot;

    private PooledList<HUDQuestListItem, Quest> questItems;

    void OnEnable()
    {
        if (questItems != null)
        {
            questItems.Clear();
        }
    }

    void Update()
    {
        var player = Universe.LocalPlayer;

        if (questItems == null)
        {
            questItems = new PooledList<HUDQuestListItem, Quest>(objectivesRoot, itemPrefab);
        }

        if (!player)
        {
            questItems.Clear();
            return;
        }

        var quests = Universe.QuestBoard.QuestsForPlayer(player);

        questItems.Refresh(quests, (i, item, quest) =>
        {
            item.Assign(quest);
        });

        header.gameObject.SetActive(questItems.Count > 0);
    }
}