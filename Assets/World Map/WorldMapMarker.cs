using UnityEngine;

public class WorldMapMarker : MonoBehaviour
{
    [SerializeField]
    private Transform billboard;

    [SerializeField]
    private TextMesh label;

    [SerializeField]
    private Transform icon;

    [SerializeField]
    private Transform tailBase;

    [SerializeField]
    private LineRenderer tail;

    [SerializeField]
    private WorldMapArea forArea;

    public WorldMapArea Area { get { return forArea; } }
    
    public static WorldMapMarker Create(WorldMapMarker prefab, WorldMapArea forArea)
    {
        var marker = (WorldMapMarker) Instantiate(prefab);
        marker.transform.SetParent(forArea.transform, false);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.localRotation = Quaternion.identity;
        marker.forArea = forArea;
        marker.gameObject.layer = LayerMask.NameToLayer("World Map");
        
        return marker;
    }

    [ContextMenu("Refresh Layout")]
    public void RefreshLayout()
    {
        label.text = forArea.name;

        var basePos = tailBase.position;
        basePos.y = 0;
        tailBase.position = basePos;

        tail.SetPosition(0, transform.position);
        tail.SetPosition(1, basePos);
    }

    private void Start()
    {
        RefreshLayout();
    }

    private void Update()
    {
        var cam = SpaceTraderConfig.WorldMap.Camera;

        billboard.transform.rotation = cam.transform.rotation;
    }
}
