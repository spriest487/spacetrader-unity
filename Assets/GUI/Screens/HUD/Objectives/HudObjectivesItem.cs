#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class HudObjectivesItem : MonoBehaviour
{
    [SerializeField]
    private MissionObjective objective;

    [SerializeField]
    private Text descriptionLabel;

    public MissionObjective Objective {
        get { return objective; }
        set { objective = value; }
    }

    void Update()
    {
        if (objective)
        {
            descriptionLabel.text = objective.Description;
            if (objective.Complete)
            {
                descriptionLabel.text += "- COMPLETE";
            }
        }
    }
}