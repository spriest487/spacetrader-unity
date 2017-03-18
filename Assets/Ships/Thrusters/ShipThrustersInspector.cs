#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

[CanEditMultipleObjects]
[CustomEditor(typeof(ShipThrusterPoint))]
class ShipThrustersInspector : Editor
{
	public override void OnInspectorGUI()
	{
        DrawDefaultInspector();

        if (!serializedObject.isEditingMultipleObjects)
        {
            var thruster = (ShipThrusterPoint)target;

            GUILayout.Label(new GUIContent(string.Format("X hull pos: " + thruster.GetXPos())));
            GUILayout.Label(new GUIContent(string.Format("Y hull pos: " + thruster.GetYPos())));
            GUILayout.Label(new GUIContent(string.Format("Z hull pos: " + thruster.GetZPos())));

            var intensity = typeof(ShipThrusterPoint).GetField("intensity", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(thruster);

            GUILayout.Label(new GUIContent(string.Format("Target intensity: " + intensity)));
        }
	}
}

#endif
