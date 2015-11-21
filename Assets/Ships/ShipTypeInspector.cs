#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShipType))]
public class ShipTypeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var shiptype = (ShipType)target;

        if (GUILayout.Button("Create instance"))
        {
            shiptype.CreateShip(Vector3.zero, Quaternion.identity);
        }
    }
}

#endif