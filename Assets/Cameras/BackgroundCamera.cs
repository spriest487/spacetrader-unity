using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundCamera : MonoBehaviour
{
    public static BackgroundCamera Current { get; private set; }

    public Camera Camera { get; private set; }

    void Start()
    {
        Camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        Debug.Assert(!Current || Current == this);
        Current = this;
    }

    void OnDisable()
    {
        Current = null;
    }

    void OnPreRender()
    {
        var mainCam = Camera.main;

        Debug.Assert(mainCam != Camera);

        transform.rotation = mainCam.transform.rotation;
        Camera.fieldOfView = mainCam.fieldOfView;
    }
}