#pragma warning disable 0649

using System;
using UnityEngine;

public class QuestList : MonoBehaviour
{
    [SerializeField]
    private Transform itemsRoot;

    [SerializeField]
    private Transform acceptedItemsRoot;

    [SerializeField]
    private QuestListItem itemPrefab;

    private PooledList<QuestListItem, Quest> questList;

    private PooledList<QuestListItem, Quest> acceptedQuestList;

    public void Close()
    {
        ScreenManager.Instance.ScreenID = ScreenID.None;
    }

    public void OnScreenActive()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (questList == null)
        {
            questList = new PooledList<QuestListItem, Quest>(itemsRoot, itemPrefab);
        }

        var station = SpaceTraderConfig.LocalPlayer.Moorable.DockedAtStation;
        var allQuests = SpaceTraderConfig.QuestBoard.QuestsAtStation(station);

        questList.Refresh(allQuests, (index, listItem, quest) => listItem.Assign(quest));

        //status of quests can change independent of which quests are displayed
        foreach (var item in questList)
        {
            item.UpdateStatus();
        }
    }

    private void OnQuestsUpdated()
    {
        Refresh();
    }
}
