using UnityEngine;
using System.Collections;

public class Moorable : MonoBehaviour
{
    [SerializeField]
    private bool moored;

    [SerializeField]
    private SpaceStation spaceStation;

    public bool Moored { get { return moored; } }
    public SpaceStation SpaceStation { get { return spaceStation; } }

    public void RequestMooring()
    {
        if (SpaceStation)
        {
            spaceStation.RequestMooring(this);
        }
        else
        {
            Debug.Log("tried to request a mooring but no station was found to moor at");
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        var mooringTrigger = collider.GetComponent<MooringTrigger>();
        if (mooringTrigger)
        {
            if (spaceStation)
            {
                Debug.LogWarning("triggered multiple spacestation mooring points, ignoring " + collider);
            }
            else
            {
                spaceStation = mooringTrigger.spaceStation;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (spaceStation && spaceStation.mooringTrigger.GetComponent<Collider>() == collider)
        {
            spaceStation = null;
        }
    }

    void Start()
    {
        moored = false;
    }

    void OnMoored(SpaceStation station)
    {
        spaceStation = station;
        moored = true;
    }

    void OnUnmoored(SpaceStation station)
    {
        spaceStation = null;
        moored = false;
    }
}
