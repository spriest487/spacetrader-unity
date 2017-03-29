using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(WaveSpawner))]
public class WaveSpawneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Auto from Children"))
        {
            foreach (WaveSpawner obj in serializedObject.targetObjects)
            {
                var groups = new List<List<ShipSpawner>>();

                //immediate children are spawn groups
                foreach (Transform child in obj.transform)
                {
                    var spawners = new List<ShipSpawner>();

                    //level below that is individual spawns
                    foreach (Transform grandchild in child)
                    {
                        var spawner = grandchild.GetComponent<ShipSpawner>();
                        if (spawner)
                        {
                            spawners.Add(spawner);
                        }
                    }

                    if (spawners.Count > 0)
                    {
                        groups.Add(spawners);
                    }
                }
                
                var serialObj = new SerializedObject(obj);
                serialObj.FindProperty("groups").arraySize = obj.transform.childCount;

                for (int g = 0; g < groups.Count; ++g)
                {
                    var serialGroup = serialObj.FindProperty("groups").GetArrayElementAtIndex(g);
                    var spawnersProp = serialGroup.FindPropertyRelative("spawners");

                    spawnersProp.arraySize = groups[g].Count;
                    for (int s = 0; s < groups[g].Count; ++s)
                    {
                        spawnersProp.GetArrayElementAtIndex(s).objectReferenceValue = groups[g][s];
                    }
                }
                serialObj.ApplyModifiedProperties();
            }
        }
    }
}
