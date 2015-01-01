﻿using UnityEngine;
using System.Collections;

public class Moorable : MonoBehaviour
{
    public SpaceStation spaceStation { get; private set; }

    public void RequestMooring()
    {
        if (spaceStation)
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
        if (spaceStation && spaceStation.mooringTrigger.collider == collider)
        {
            spaceStation = null;
        }
    }
}
