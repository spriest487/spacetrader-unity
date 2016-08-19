#pragma warning disable 0649

using UnityEngine;

public class CutsceneCameraRig : MonoBehaviour
{
    [SerializeField]
    private Transform view;

    [SerializeField]
    private Transform focus;

    public Vector3 View
    {
        get { return view.position; }
    }

    public Vector3 Focus
    {
        get { return focus.position; }
    }
}