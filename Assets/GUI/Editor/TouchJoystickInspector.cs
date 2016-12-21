using UnityEditor;

[CustomEditor(typeof(TouchJoystick))]
public class TouchJoystickInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var joystick = (TouchJoystick) target;

        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Value", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(TouchJoystick.Value.ToString("F3"));
        }

        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Available", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(TouchJoystick.Available.ToString());
        }

        using (var scope = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Finger ID", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(joystick.FingerID.HasValue? joystick.FingerID.ToString() : "--");
        }
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}
