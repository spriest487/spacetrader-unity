using UnityEngine;

public class CockpitView : MonoBehaviour
{
    // private FollowCamera playerCam;

    // [SerializeField]
    // private Transform hmdPositionOffset;

    // [SerializeField]
    // private Transform hmdRotationOffset;

    public static CockpitView Create(CockpitView prefab, FollowCamera playerCam)
    {
        var cockpit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        //cockpit.playerCam = playerCam;

        return cockpit;
    }

    // private void OnEnable()
    // {
    //     playerCam.OnHMDReset += OnHMDReset;
    // }

    // private void OnDisable()
    // {
    //     playerCam.OnHMDReset -= OnHMDReset;
    // }

    // private void OnHMDReset(Vector3 position, Quaternion rotation)
    // {
    //     hmdPositionOffset.localPosition = position;
    //     hmdRotationOffset.localRotation = rotation;
    // }
}
