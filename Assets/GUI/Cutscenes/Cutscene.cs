using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CutscenePage
{
    public string Text;
    public string Speaker;
}

public class Cutscene : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Cutscene")]
    public static void CreateNewCutscene()
    {
        ScriptableObjectUtility.CreateAsset<Cutscene>();
    }
#endif

    [SerializeField]
    private List<CutscenePage> pages;

    [SerializeField]
    private int page = 0;

    public CutscenePage CurrentPage
    {
        get { return page < pages.Count ? pages[page] : null; }
    }

    public void Next()
    {
        page += 1;
    }
}

#if UNITY_EDITOR 

[UnityEditor.CustomEditor(typeof(Cutscene))]
public class CutsceneInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Play now"))
        {
            ScreenManager.Instance.PlayCutscene(target as Cutscene);
        }
    }
}

#endif
