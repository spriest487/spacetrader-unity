#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GUIElement))]
public class WorldMapScreen : MonoBehaviour
{
    new GUIElement guiElement;
    
    void Awake()
    {
        guiElement = GetComponent<GUIElement>();
    }

    void OnEnable()
    {        
        var defaultCameraDist = new Vector3(0, 20, -40);
        var currentArea = SpaceTraderConfig.WorldMap.GetCurrentArea();

        var mapCam = SpaceTraderConfig.WorldMap.Camera;
        mapCam.transform.position = currentArea.transform.position + defaultCameraDist;
        mapCam.transform.rotation = Quaternion.LookRotation(-mapCam.transform.position.normalized, Vector3.up);

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
    }
}