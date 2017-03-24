using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Hitpoints))]
public class HitpointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying && GUILayout.Button("Destroy"))
        {
            foreach (Hitpoints hp in serializedObject.targetObjects)
            {
                hp.TakeDamage(1000000);
            }
        }
    }
}
