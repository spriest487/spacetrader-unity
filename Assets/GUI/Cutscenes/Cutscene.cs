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

[CreateAssetMenu(menuName = "SpaceTrader/Cutscene")]
public class Cutscene : ScriptableObject
{
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
            GUIController.Current.CutsceneOveray.PlayCutscene(target as Cutscene);
        }
    }
}

#endif
