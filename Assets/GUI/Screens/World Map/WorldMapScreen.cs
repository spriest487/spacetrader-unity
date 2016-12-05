#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(GUIElement))]
public class WorldMapScreen : MonoBehaviour
{
    new GUIElement guiElement;

    [SerializeField]
    Camera mapCamera;

    Vector3 camOffset;

    void Awake()
    {
        guiElement = GetComponent<GUIElement>();
    }

    void OnEnable()
    {
        var currentArea = SpaceTraderConfig.WorldMap.GetCurrentArea();
        camOffset = currentArea.transform.position + new Vector3(0, 20, -40);

        mapCamera.transform.position = camOffset;
        mapCamera.transform.rotation = Quaternion.LookRotation(-camOffset.normalized, Vector3.up);

        foreach (var marker in SpaceTraderConfig.WorldMap.Markers)
        {
            marker.RefreshLayout();
        }
    }

    void BlackedOut()
    {
        SpaceTraderConfig.WorldMap.Visible = guiElement.Activated;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)
            && SpaceTraderConfig.LocalPlayer
            && SpaceTraderConfig.LocalPlayer.Ship)
        {
            var pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            var ray = SpaceTraderConfig.WorldMap.Camera.ScreenPointToRay(pos);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("World Map")))
            {
                //the only things on this layer with colliders should be markers
                var mapArea = hit.collider.GetComponent<WorldMapMarker>().Area;

                SpaceTraderConfig.LocalPlayer.Ship.Target = mapArea.GetComponent<Targetable>();

                Debug.Log("targetted area " + mapArea);
            }
        }

        float eastWest = Input.GetAxis("yaw");
        float northSouth = Input.GetAxis("pitch");

        const float PAN_SPEED = 50;

        camOffset += PAN_SPEED * Time.deltaTime * new Vector3(eastWest, 0, northSouth);
        var lerped = Vector3.Lerp(mapCamera.transform.position, camOffset, 2 * Time.deltaTime);

        var camXform = mapCamera.transform;
        camXform.position = lerped;
    }
}