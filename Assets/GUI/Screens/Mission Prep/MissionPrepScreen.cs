using UnityEngine;
using UnityEngine.UI;

public class MissionPrepScreen : MonoBehaviour {
    public static MissionPrepScreen Instance { get; private set; }

    [SerializeField]
    private MissionDefinition currentMission;

    [SerializeField]
    private Button readyButton;

    public MissionDefinition CurrentMission { get { return currentMission; } }

    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogWarning("more than one mission manager singleton, destroying");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void OnDisable()
    {
        if (Instance != this)
        {
            Debug.LogWarning("unsetting mission manager singleton - wrong instance set");
        }
        else
        {
            Instance = null;
        }
    }

    void Update()
    {
        readyButton.interactable = true;
    }
}
