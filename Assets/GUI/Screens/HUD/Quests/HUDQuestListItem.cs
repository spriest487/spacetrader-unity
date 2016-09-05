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
        if (quest.Done)
        {
            descriptionLabel.text += "- COMPLETE";
        }
    }

    public void Assign(Quest quest)
    {
        this.quest = quest;
        Update();
    }
}