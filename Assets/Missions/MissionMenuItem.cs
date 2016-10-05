#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class MissionMenuItem : MonoBehaviour
{
    [SerializeField]
    private MissionDefinition missionDefinition;

    [SerializeField]
    private Text missionNameText;

    [SerializeField]
    private Button button;    

    public MissionDefinition MissionDefinition { get { return missionDefinition; } set { missionDefinition = value; } }

    public Button Button { get { return button; } }
    
    void Start()
    {
        if (!missionDefinition || !button)
        {
            throw new UnityException("no definition for mission item");
        }

        if (missionNameText)
        {
            missionNameText.text = missionDefinition.MissionName;
        }

        var menu = GetComponentInParent<MissionsMenu>();
        if (!menu)
        {
            throw new UnityException("mission menuitem not a child of a mission menu");
        }

        if (button)
        {
            button.onClick.AddListener(() => {
                menu.SelectMission(MissionDefinition);
            });
        }
    }
}