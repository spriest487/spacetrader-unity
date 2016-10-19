#pragma warning disable 0649

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
        var docked = !!SpaceTraderConfig.LocalPlayer.Moorable.DockedAtStation;
        var myQuest = SpaceTraderConfig.LocalPlayer == SpaceTraderConfig.QuestBoard.OwnerOf(quest);

        completeButton.gameObject.SetActive(docked && myQuest && quest.Done);
        acceptButton.gameObject.SetActive(docked && !myQuest && !quest.Done);
        abandonButton.gameObject.SetActive(myQuest);
    }

    public void AcceptQuest()
    {
        SpaceTraderConfig.QuestBoard.AcceptQuest(SpaceTraderConfig.LocalPlayer, quest);

        ScreenManager.Instance.BroadcastScreenMessage(ScreenID.Quests, "OnQuestsUpdated", this);
    }

    public void AbandonQuest()
    {
        SpaceTraderConfig.QuestBoard.CancelQuest(quest);

        ScreenManager.Instance.BroadcastScreenMessage(ScreenID.Quests, "OnQuestsUpdated", this);
    }

    public void FinishQuest()
    {
        SpaceTraderConfig.QuestBoard.FinishQuest(quest);

        ScreenManager.Instance.BroadcastScreenMessage(ScreenID.Quests, "OnQuestsUpdated", this);
    }
}
