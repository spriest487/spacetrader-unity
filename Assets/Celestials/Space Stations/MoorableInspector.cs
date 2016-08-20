#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Moorable))]
public class MoorableInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var moorable = (Moorable)target;

        GUI.enabled = false;
        EditorGUILayout.ObjectField(moorable.LocalStation, typeof(SpaceStation), true);
        GUI.enabled = true;
    }
}

#endif