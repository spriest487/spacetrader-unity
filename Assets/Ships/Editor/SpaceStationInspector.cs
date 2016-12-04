using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(SpaceStation))]
public class SpaceStationInspector : Editor {

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var station = target as SpaceStation;
        if (!station || !Application.isPlaying)
        {
            return;
        }

        EditorGUILayout.LabelField("Quests", EditorStyles.boldLabel);

        var quests = SpaceTraderConfig.QuestBoard.QuestsAtStation(station);
        foreach (var quest in quests)
        {
            var content = new GUIContent(quest.name, quest.Description);
            EditorGUILayout.LabelField(content);
        }
    }
}
