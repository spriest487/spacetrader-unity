using System.Collections;
using UnityEngine;

public class ScreenManager : MonoBehaviour {
    public enum IngameState
    {
        Docked,
        Flight,
    }

    public GameObject[] dockedScreen;
    public GameObject[] flightScreen;

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
                    foreach (var dockedObj in dockedScreen)
                    {
                        dockedObj.SetActive(true);
                    }

                    foreach (var flightObj in flightScreen)
                    {
                        flightObj.SetActive(false);
                    }
                    
                    break;
                }
            case IngameState.Flight:
                {
                    foreach (var dockedObj in dockedScreen)
                    {
                        dockedObj.SetActive(false);
                    }

                    foreach (var flightObj in flightScreen)
                    {
                        flightObj.SetActive(true);
                    }

                    break;
                }
        }
    }

    void Start()
    {
        Apply();
    }
}