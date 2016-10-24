using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundCamera : MonoBehaviour
{
    public static BackgroundCamera Current { get; private set; }

    public Camera Camera { get; private set; }

    void Start()
    {
        Camera = GetComponent<Camera>();

        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        Debug.Assert(!Current || Current == this);
        Current = this;
    }

    void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;
    }

    void OnPreRender()
    {
        var mainCam = FollowCamera.Current;

        Debug.Assert(mainCam, "main camera must exist");
        Debug.Assert(mainCam != Camera);

        transform.rotation = mainCam.transform.rotation;
        Camera.fieldOfView = mainCam.Camera.fieldOfView;
    }
}