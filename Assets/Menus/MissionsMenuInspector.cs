#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MissionsMenu))]
class MissionsMenuInspector : Editor
{
    private MissionsMenu missions;

    public void OnEnable()
    {
        missions = (MissionsMenu)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

#endif