#pragma warning disable 0649

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestList : MonoBehaviour
{
    [Header("Available Quests")]

    [SerializeField]
    private Transform itemsRoot;

    [SerializeField]
    private Text availableHeader;

    [Header("Accepted Quests")]

    [SerializeField]
    private Text acceptedHeader;

    [SerializeField]
    private Transform acceptedRoot;

    [Header("Prefabs")]

    [SerializeField]
    private QuestListItem itemPrefab;

    private PooledList<QuestListItem, Quest> questList;
    private PooledList<QuestListItem, Quest> acceptedQuestList;

    public void Close()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.None);
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

        if (acceptedQuestList == null)
        {
            acceptedQuestList = new PooledList<QuestListItem, Quest>(acceptedRoot, itemPrefab);
        }

        var station = SpaceTraderConfig.LocalPlayer.Moorable.DockedAtStation;
        var myQuests = SpaceTraderConfig.QuestBoard.QuestsForPlayer(PlayerShip.LocalPlayer);
        var availableQuests = SpaceTraderConfig.QuestBoard.QuestsAtStation(station)
            .Except(myQuests);

        if (station)
        {
            availableHeader.gameObject.SetActive(true);

            questList.Refresh(availableQuests, (index, listItem, quest) => 
                listItem.Assign(quest));

            if (questList.Count == 0)
            {
                availableHeader.text = "No jobs available at " + station.name;
                itemsRoot.gameObject.SetActive(false);
            }
            else
            {
                availableHeader.text = "Jobs available at " + station.name;
                itemsRoot.gameObject.SetActive(true);
            }
        }
        else
        {
            questList.Clear();
            availableHeader.gameObject.SetActive(false);
            itemsRoot.gameObject.SetActive(false);
        }

        acceptedQuestList.Refresh(myQuests, (index, listItem, quest) =>
            listItem.Assign(quest));
        bool haveAcceptedQuests = acceptedQuestList.Count > 0;

        acceptedHeader.gameObject.SetActive(haveAcceptedQuests);
        acceptedRoot.gameObject.SetActive(haveAcceptedQuests);

        //status of quests can change independent of which quests are displayed
        foreach (var item in questList)
        {
            item.UpdateStatus();
        }
        foreach (var item in acceptedQuestList)
        {
            item.UpdateStatus();
        }
    }

    private void OnQuestsUpdated()
    {
        Refresh();
    }
}
