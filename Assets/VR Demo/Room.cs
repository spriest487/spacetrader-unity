#pragma warning disable 0649

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

    private void Awake()
    {
        UpdateVRSetting();
    }

    private void OnEnable()
    {
        if (Universe.Instance)
        {
            Universe.OnPrefsSaved += UpdateVRSetting;
        }
    }

    private void OnDisable()
    {
        if (Universe.Instance)
        {
            Universe.OnPrefsSaved -= UpdateVRSetting;
        }
    }

    private void UpdateVRSetting()
    {
        vrCamera.gameObject.SetActive(VRSettings.enabled);
        floor.gameObject.SetActive(VRSettings.enabled);
        ceiling.gameObject.SetActive(false);
    }
}
