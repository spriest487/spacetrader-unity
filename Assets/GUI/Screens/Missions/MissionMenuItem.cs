#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class MissionMenuItem : MonoBehaviour
{
    [Header("UI")]

    [SerializeField]
    private Text missionNameText;

    [SerializeField]
    private Image missionImage;

    [SerializeField]
    private Button button;

    [Header("Runtime")]

    [SerializeField]
    private MissionDefinition missionDefinition;

    public MissionDefinition MissionDefinition { get { return missionDefinition; } }

    public Button Button { get { return button; } }

    public static MissionMenuItem Create(MissionMenuItem prefab, MissionDefinition missionDef)
    {
        var item = Instantiate(prefab);

        item.missionDefinition = missionDef;
        item.missionNameText.text = missionDef.name.ToUpper();
        item.missionImage.sprite = missionDef.Image;

        return item;
    }

    public void SelectMission()
    {
        GetComponentInParent<MissionsMenu>().SelectMission(missionDefinition);
    }
}