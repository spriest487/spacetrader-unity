#pragma warning disable 0649

using UnityEngine;

public class WorldMapMarker : MonoBehaviour
{
    [SerializeField]
    private Transform youAreHereMarker;

    [SerializeField]
    private Transform selectedMarker;

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
        if (forArea)
        {
            label.text = forArea.name;
        }

        var basePos = tailBase.position;
        basePos.y = 0;
        tailBase.position = basePos;

        tail.SetPosition(0, transform.position);
        tail.SetPosition(1, basePos);

        var labelPos = label.transform.localPosition;
        float textOffset = icon.GetComponent<SpriteRenderer>().bounds.extents.y / 2;
        bool upsideDown = transform.position.y < 0;
        
        labelPos.y = upsideDown ? -textOffset : textOffset;
        label.transform.localPosition = labelPos;


        label.anchor = upsideDown ? TextAnchor.UpperCenter : TextAnchor.LowerCenter;
    }

    private void Start()
    {
        RefreshLayout();
    }

    private void Update()
    {
        var map = SpaceTraderConfig.WorldMap;
        var player = SpaceTraderConfig.LocalPlayer;
        var cam = map.Camera;

        billboard.transform.rotation = cam.transform.rotation;
        tailBase.transform.rotation = cam.transform.rotation;

        youAreHereMarker.gameObject.SetActive(map.GetCurrentArea() == forArea);
        selectedMarker.gameObject.SetActive(player
            && player.Ship
            && player.Ship.Target
            && player.Ship.Target.ActionOnActivate == forArea);
    }
}
