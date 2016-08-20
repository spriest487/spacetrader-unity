using UnityEngine;

public class MatchCameraRotation : MonoBehaviour
{
    void OnPreRender()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}