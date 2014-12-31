#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Starfield))]
public class StarfieldInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var starfield = (Starfield) target;
		if (GUILayout.Button("Rebuild"))
		{
			starfield.Start();
		}
	}
}

#endif
