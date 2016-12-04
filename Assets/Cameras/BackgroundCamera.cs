#pragma warning disable 0649

using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    public static BackgroundCamera Current { get; private set; }

    public Camera Camera { get; private set; }

    [SerializeField]
    private FollowCamera matchCamera;

    [SerializeField]
    private Transform hmdOffset;

    void Awake()
    {
        Camera = GetComponentInChildren<Camera>();
    }

    void OnWorldMapVisibilityChanged(bool visible)
    {
        Camera.enabled = !visible;
    }

    void OnEnable()
    {
        Debug.Assert(!Current || Current == this);
        Current = this;

        SpaceTraderConfig.WorldMap.OnVisibilityChanged += OnWorldMapVisibilityChanged;
        matchCamera.OnHMDReset += OnHMDReset;
        Camera.onPreRender += MatchOnPreRender;
    }

    void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;

        matchCamera.OnHMDReset -= OnHMDReset;
        Camera.onPreRender -= MatchOnPreRender;

        if (SpaceTraderConfig.Instance && SpaceTraderConfig.WorldMap)
        {
            SpaceTraderConfig.WorldMap.OnVisibilityChanged -= OnWorldMapVisibilityChanged;
        }
    }

    private void OnHMDReset(Vector3 position, Quaternion rotation)
    {
        hmdOffset.localRotation = rotation;
    }

    void MatchOnPreRender(Camera renderingCam)
    {
        if (renderingCam == Camera)
        {
            transform.rotation = matchCamera.transform.rotation;
            Camera.fieldOfView = matchCamera.Camera.fieldOfView;
        }
    }
}