#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ScreenManager))]
[ExecuteInEditMode]
class ScreenManagerInspector : Editor
{
    private ScreenManager screenManager;
    private ScreenManager.IngameState selectedIngameState;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Ingame state: " +screenManager.ingameState);

        selectedIngameState = (ScreenManager.IngameState)EditorGUILayout.EnumPopup("New ingame state", selectedIngameState);

        if (GUILayout.Button("Apply"))
        {
            screenManager.ingameState = selectedIngameState;
        }
    }

    public void OnEnable()
    {
        screenManager = (ScreenManager)target;
        selectedIngameState = screenManager.ingameState;
    }
}

#endif