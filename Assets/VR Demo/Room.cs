using UnityEngine;
using UnityEngine.VR;

public class Room : MonoBehaviour
{
    [SerializeField]
    private Transform floor;

    [SerializeField]
    private Transform ceiling;

    [SerializeField]
    private Camera vrCamera;

    [SerializeField]
    private Camera overheadCamera;

    public Camera VRCamera { get { return vrCamera; } }
    public Camera OverheadCamera { get { return overheadCamera; } }
    
    private void Update()
    {
        vrCamera.gameObject.SetActive(VRSettings.enabled);
        floor.gameObject.SetActive(VRSettings.enabled);
        ceiling.gameObject.SetActive(false);
    }
}
