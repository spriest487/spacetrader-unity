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

    private HUD hud;

    private void Awake()
    {
        var gui = GetComponentInParent<GUIController>();
        hud = gui.GetComponentInChildren<HUD>(true);
    }

    public void Start()
    {
        vrToggle.gameObject.SetActive(VRSettings.supportedDevices.Any());
        vrToggle.isOn = VRSettings.enabled;

        touchControlsToggle.isOn = hud.TouchControlsEnabled;
    }

    public void Save()
    {
        VRSettings.enabled = vrToggle.isOn;
        hud.TouchControlsEnabled = touchControlsToggle.isOn;

        SpaceTraderConfig.Instance.SavePrefs();

        GetComponent<GUIElement>().Dismiss();
    }
}
