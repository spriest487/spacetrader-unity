using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(GUIController))]
public class GUIControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var gui = (GUIController)target;
        var guiClass = typeof(GUIController);

        using (var horizontal = new EditorGUILayout.HorizontalScope())
        {
            var activeScreenMethod = guiClass.GetMethod("FindActiveScreen", BindingFlags.NonPublic | BindingFlags.Instance);

            GUIScreen activeScreen = null;
            if (Application.isPlaying)
            {
                activeScreen = (GUIScreen)activeScreenMethod.Invoke(gui, null);
            }

            var screenID = activeScreen ? activeScreen.ID.ToString() : "--";
            EditorGUILayout.LabelField("Active Screen", screenID);
        }

        DrawDefaultInspector();

        EditorGUILayout.LabelField("Active Transition", EditorStyles.boldLabel);

        var transitionField = guiClass.GetField("activeTransition", BindingFlags.NonPublic | BindingFlags.Instance);
        var transition = (GUITransition)transitionField.GetValue(gui);

        using (var horizontal = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("To Screen");

            if (transition == null)
            {
                EditorGUILayout.LabelField("--");
            }
            else
            {
                var transClass = transition.GetType();
                var toScreenField = transClass.GetField("toScreen", BindingFlags.Instance | BindingFlags.Public);
                string toScreen = toScreenField.GetValue(transition).ToString();

                EditorGUILayout.LabelField(toScreen);
            }
        }

        using (var horizontal = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Progress");
            string progress = transition != null ? transition.Progress.ToString() : "--";
            EditorGUILayout.LabelField(progress);
        }

        EditorGUILayout.LabelField("Loading Transition", EditorStyles.boldLabel);

        var loadingField = guiClass.GetField("loadingTransition", BindingFlags.NonPublic | BindingFlags.Instance);
        var loading = (GUITransition)loadingField.GetValue(gui);
        
        using (var horizontal = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Progress");
            string progress = loading != null ? loading.Progress.ToString() : "--";
            EditorGUILayout.LabelField(progress);
        }
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}
