#pragma warning disable 0649

using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    public static BackgroundCamera Current { get; private set; }

    public Camera Camera { get; private set; }

    [SerializeField]
    private FollowCamera matchCamera;

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
        Camera.onPreRender += MatchOnPreRender;

        Debug.Assert(!Current || Current == this);
        Current = this;

        SpaceTraderConfig.WorldMap.OnVisibilityChanged += OnWorldMapVisibilityChanged;
    }

    void OnDisable()
    {
        Camera.onPreRender -= MatchOnPreRender;

        Debug.Assert(Current == this);
        Current = null;

        if (SpaceTraderConfig.Instance && SpaceTraderConfig.WorldMap)
        {
            SpaceTraderConfig.WorldMap.OnVisibilityChanged -= OnWorldMapVisibilityChanged;
        }
    }

    void MatchOnPreRender(Camera renderingCam)
    {
        transform.rotation = matchCamera.transform.rotation;
        Camera.fieldOfView = matchCamera.Camera.fieldOfView;
    }
}