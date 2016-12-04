#pragma warning disable 0649

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestList : MonoBehaviour
{
    [Serializable]
    public struct QuestGroup
    {
        public Transform ItemsRoot;
        public Transform EmptyMessage;
        public Text Header;

        public void Refresh(PooledList<QuestListItem, Quest> itemsList)
        {
            EmptyMessage.gameObject.SetActive(itemsList.Count == 0);

            //status of quests can change independent of which quests are displayed
            foreach (var item in itemsList)
            {
                item.UpdateStatus();
            }
        }

        public void SetActive(bool active)
        {
            ItemsRoot.gameObject.SetActive(active);
            Header.gameObject.SetActive(active);
        }
    }

    [SerializeField]
    private QuestGroup availableQuests;

    [SerializeField]
    private QuestGroup acceptedQuests;

    [Header("Prefabs")]

    [SerializeField]
    private QuestListItem itemPrefab;

    private PooledList<QuestListItem, Quest> questList;
    private PooledList<QuestListItem, Quest> acceptedQuestList;

    public void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (questList == null)
        {
            questList = new PooledList<QuestListItem, Quest>(availableQuests.ItemsRoot, itemPrefab);
        }

        if (acceptedQuestList == null)
        {
            acceptedQuestList = new PooledList<QuestListItem, Quest>(acceptedQuests.ItemsRoot, itemPrefab);
        }

        var player = SpaceTraderConfig.LocalPlayer;

        if (player)
        {
            var station = player.Moorable.DockedAtStation;
            var myQuests = SpaceTraderConfig.QuestBoard.QuestsForPlayer(player);
            var questsNotAccepted = SpaceTraderConfig.QuestBoard.QuestsAtStation(station)
                .Except(myQuests);

            availableQuests.SetActive(station);
            if (station)
            {
                questList.Refresh(questsNotAccepted, (index, listItem, quest) =>
                    listItem.Assign(quest));
            }
            else
            {
                questList.Clear();
            }

            acceptedQuestList.Refresh(myQuests, (index, listItem, quest) =>
                listItem.Assign(quest));
        }
        else
        {
            questList.Clear();
            acceptedQuestList.Clear();
        }

        availableQuests.Refresh(questList);
        acceptedQuests.Refresh(acceptedQuestList);
    }

    private void OnQuestsUpdated()
    {
        Refresh();
    }
}
