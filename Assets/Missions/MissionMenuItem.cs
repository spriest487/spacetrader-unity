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

    public MissionDefinition MissionDefinition { get { return missionDefinition; } }

    public Button Button { get { return button; } }
    
    public static MissionMenuItem Create(MissionMenuItem prefab, MissionDefinition missionDef)
    {
        var item = Instantiate(prefab);

        item.missionDefinition = missionDef;
        item.missionNameText.text = missionDef.name;

        return item;
    }

    public void SelectMission()
    {
        GetComponentInParent<MissionsMenu>().SelectMission(missionDefinition);
    }
}