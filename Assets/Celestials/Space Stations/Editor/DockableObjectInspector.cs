#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(DockableObject))]
public class DockableObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var dockable = (DockableObject)target;

        GUI.enabled = false;
        EditorGUILayout.ObjectField(dockable.LocalStation, typeof(SpaceStation), true);
        GUI.enabled = true;
    }
}

#endif