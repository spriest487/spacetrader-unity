#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundCamera : MonoBehaviour
{
    public static BackgroundCamera Current { get; private set; }

    public Camera Camera { get; private set; }

    [SerializeField]
    private FollowCamera matchCamera;

    void Awake()
    {
        Camera = GetComponent<Camera>();
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
    }

    void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;

        if (SpaceTraderConfig.Instance && SpaceTraderConfig.WorldMap)
        {
            SpaceTraderConfig.WorldMap.OnVisibilityChanged -= OnWorldMapVisibilityChanged;
        }
    }

    void OnPreRender()
    {
        Debug.Assert(matchCamera, "main camera must exist");
        Debug.Assert(matchCamera != Camera);

        transform.rotation = matchCamera.transform.rotation;
        Camera.fieldOfView = matchCamera.Camera.fieldOfView;
    }
}