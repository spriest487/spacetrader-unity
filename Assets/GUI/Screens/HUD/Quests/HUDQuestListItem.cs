#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class HUDQuestListItem : MonoBehaviour
{
    [SerializeField]
    private Quest quest;

    [SerializeField]
    private Text descriptionLabel;

    public Quest Quest
    {
        get { return quest; }
        set { quest = value; }
    }

    void Update()
    {
        if (!quest)
        {
            return;
        }

        descriptionLabel.text = quest.Description;

        switch (quest.Status)
        {
            case QuestStatus.Completed:
                descriptionLabel.text += "- COMPLETE";
                descriptionLabel.color = Color.green;
                break;
            case QuestStatus.Failed:
                descriptionLabel.text += "- FAILED";
                descriptionLabel.color = Color.red;
                break;
            default:
                descriptionLabel.color = Color.white;
                break;
        }
    }

    public void Assign(Quest quest)
    {
        this.quest = quest;
        Update();
    }
}