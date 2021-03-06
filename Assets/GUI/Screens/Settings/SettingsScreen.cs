#pragma warning disable 0649

using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using System.Linq;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField]
    private Toggle vrToggle;

    [SerializeField]
    private Toggle touchControlsToggle;

    public void Start()
    {
        vrToggle.gameObject.SetActive(VRSettings.supportedDevices.Any());
        vrToggle.isOn = VRSettings.enabled;

        touchControlsToggle.isOn = Universe.TouchControlsEnabled;
    }

    public void Save()
    {
        VRSettings.enabled = vrToggle.isOn;
        Universe.TouchControlsEnabled = touchControlsToggle.isOn;

        Universe.SavePrefs();

        GetComponent<GUIElement>().Dismiss();
    }
}
