using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using System.Linq;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField]
    private Toggle vrToggle;

    public void OnEnable()
    {
        vrToggle.gameObject.SetActive(VRSettings.supportedDevices.Any());
        vrToggle.isOn = VRSettings.enabled;
    }

    public void Save()
    {
        VRSettings.enabled = vrToggle.isOn;
        SpaceTraderConfig.SavePrefs();

        GetComponentInParent<GUIController>().DismissActive();
    }
}
