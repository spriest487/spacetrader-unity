#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//[CustomEditor(typeof(ScreenManager))]
//[ExecuteInEditMode]
class ScreenManagerInspector : Editor
{
    private ScreenManager screenManager;
    private ScreenManager.ScreenState selectedIngameState;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //screenManager.State = (ScreenManager.ScreenState) EditorGUILayout.EnumPopup("Ingame state", screenManager.State);
        EditorGUILayout.LabelField("Ingame state", screenManager.State.ToString());
        screenManager.MenuState = EditorGUILayout.Toggle("Menu state", screenManager.MenuState);

        selectedIngameState = (ScreenManager.ScreenState)EditorGUILayout.EnumPopup("New ingame state", selectedIngameState);
    }

    public void OnEnable()
    {
        screenManager = (ScreenManager)target;
        selectedIngameState = screenManager.State;
    }
}

#endif