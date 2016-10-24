using System;
using UnityEngine;

public class WorldMapScreen : MonoBehaviour
{
    void OnScreenActive()
    {
        /* disable the camera component, not the whole object - or the Current
         reference will be unset */
        FollowCamera.Current.Camera.enabled = false;
        BackgroundCamera.Current.Camera.enabled = false;

        var mapCam = SpaceTraderConfig.WorldMap.Camera;
        
        mapCam.enabled = true;

        var defaultCameraDist = new Vector3(0, 20, -40);
        var currentArea = SpaceTraderConfig.WorldMap.GetCurrentArea();

        mapCam.transform.position = currentArea.transform.position + defaultCameraDist;
        mapCam.transform.rotation = Quaternion.LookRotation(-mapCam.transform.position.normalized, Vector3.up);

        foreach (var marker in SpaceTraderConfig.WorldMap.Markers)
        {
            marker.RefreshLayout();
        }
    }

    private void OnScreenInactive()
    {
        FollowCamera.Current.Camera.enabled = true;
        BackgroundCamera.Current.Camera.enabled = true;

        SpaceTraderConfig.WorldMap.Camera.enabled = false;
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