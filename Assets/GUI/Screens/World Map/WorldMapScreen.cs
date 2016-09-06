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

        SpaceTraderConfig.WorldMap.Camera.enabled = true;
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