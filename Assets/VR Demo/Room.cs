#pragma warning disable 0649

using UnityEngine;
using UnityEngine.VR;

public class Room : MonoBehaviour
{
    const float HandIdleSleepTime = 5;

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

    private HandController[] hands;

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

        //disable all hands initially until they move
        hands = GetComponentsInChildren<HandController>();
        foreach (var hand in hands)
        {
            hand.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (Universe.Instance)
        {
            Universe.OnPrefsSaved -= UpdateVRSetting;
        }
    }

    private void LateUpdate()
    {
        foreach (var hand in hands)
        {
            if (hand.gameObject.activeSelf)
            {
                if (hand.IdleTime > HandIdleSleepTime)
                {
                    hand.gameObject.SetActive(false);
                }
            }
            else
            {
                if (hand.HasMoved)
                {
                    hand.gameObject.SetActive(true);
                }
            }
        }
    }

    private void UpdateVRSetting()
    {
        vrCamera.gameObject.SetActive(VRSettings.enabled);
        floor.gameObject.SetActive(VRSettings.enabled);
        ceiling.gameObject.SetActive(false);
    }
}
