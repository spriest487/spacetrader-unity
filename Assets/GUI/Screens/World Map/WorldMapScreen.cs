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
    }

    public void Hide()
    {
        FollowCamera.Current.Camera.enabled = true;
        BackgroundCamera.Current.Camera.enabled = true;

        SpaceTraderConfig.WorldMap.Camera.enabled = false;

        ScreenManager.Instance.ScreenID = ScreenID.None;
    }

    public void Update()
    {
        if (Input.GetButtonDown("map"))
        {
            Hide();
        }
    }
}