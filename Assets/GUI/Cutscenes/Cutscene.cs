#pragma warning disable 0649

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
    private CutsceneCameraRig cameraRigPrefab;

    [SerializeField]
    [HideInInspector]
    private int page = 0;

    [SerializeField]
    [HideInInspector]
    private CutsceneCameraRig cameraRig;

    public CutscenePage CurrentPage
    {
        get { return page < pages.Count ? pages[page] : null; }
    }

    public CutsceneCameraRig CameraRig
    {
        get { return cameraRig; }
    }

    public void Start()
    {
        if (cameraRigPrefab)
        {
            cameraRig = Instantiate(cameraRigPrefab);
        }
        
        page = 0;
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
