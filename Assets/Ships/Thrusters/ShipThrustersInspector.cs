#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ShipThrusterPoint))]
class ShipThrustersInspector : Editor
{
	public override void OnInspectorGUI()
	{
		var thruster = (ShipThrusterPoint) target;

		base.OnInspectorGUI();

		GUILayout.Label(new GUIContent(string.Format("X hull pos: " + thruster.GetXPos())));
		GUILayout.Label(new GUIContent(string.Format("Y hull pos: " + thruster.GetYPos())));
		GUILayout.Label(new GUIContent(string.Format("Z hull pos: " + thruster.GetZPos())));

		GUILayout.Label(new GUIContent(string.Format("Target intensity: " + thruster.GetIntensity())));
	}
}

#endif
