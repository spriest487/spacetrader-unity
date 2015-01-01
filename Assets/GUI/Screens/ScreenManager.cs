using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour {
    public enum IngameState
    {
        Docked,
        Flight,
    }

    public GameObject dockedScreen;
    public GameObject flightScreen;

    private IngameState _ingameState = IngameState.Flight;
    public IngameState ingameState
    {
        get
        {
            return _ingameState;
        }
        set
        {
            _ingameState = value;
            Apply();
        }
    }

    public void Apply()
    {
        switch (ingameState)
        {
            case IngameState.Docked:
                {
                    dockedScreen.SetActive(true);
                    flightScreen.SetActive(false);
                    break;
                }
            case IngameState.Flight:
                {
                    dockedScreen.SetActive(false);
                    flightScreen.SetActive(true);
                    break;
                }
        }
    }

    void Start()
    {
        Apply();
    }
}