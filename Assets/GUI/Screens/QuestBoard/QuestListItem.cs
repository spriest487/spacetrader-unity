﻿#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class QuestListItem : MonoBehaviour
{
    [SerializeField]
    private Quest quest;

    [Header("UI")]

    [SerializeField]
    private Text title;

    [SerializeField]
    private Text description;

    [SerializeField]
    private Button acceptButton;

    [SerializeField]
    private Button completeButton;

    [SerializeField]
    private Button abandonButton;

    [SerializeField]
    private Text xpLabel;

    [SerializeField]
    private Text moneyLabel;

    public void Assign(Quest quest)
    {
        this.quest = quest;

        title.text = quest.name;
        description.text = quest.Description;
        xpLabel.text = string.Format("{0:N0} XP", quest.XPReward);
        moneyLabel.text = Market.FormatCurrency(quest.MoneyReward);
    }

    public void UpdateStatus()
    {
        var docked = !!Universe.LocalPlayer.Dockable.DockedAtStation;
        var myQuest = Universe.LocalPlayer == Universe.QuestBoard.OwnerOf(quest);

        completeButton.gameObject.SetActive(docked && myQuest && quest.Status == QuestStatus.Completed);
        acceptButton.gameObject.SetActive(docked && !myQuest && quest.Status == QuestStatus.NotAccepted);
        abandonButton.gameObject.SetActive(myQuest);
    }

    public void AcceptQuest()
    {
        Universe.QuestBoard.AcceptQuest(Universe.LocalPlayer, quest);

        GUIController.Current.BroadcastMessage("OnQuestsUpdated", this, SendMessageOptions.DontRequireReceiver);
    }

    public void AbandonQuest()
    {
        Universe.QuestBoard.CancelQuest(quest);

        GUIController.Current.BroadcastMessage("OnQuestsUpdated", this, SendMessageOptions.DontRequireReceiver);
    }

    public void FinishQuest()
    {
        Universe.QuestBoard.FinishQuest(quest);

        GUIController.Current.BroadcastMessage("OnQuestsUpdated", this, SendMessageOptions.DontRequireReceiver);
    }
}
