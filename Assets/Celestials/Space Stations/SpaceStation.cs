using UnityEngine;
using System.Collections.Generic;
using System;

public class SpaceStation : MonoBehaviour
{
    [SerializeField]
    private MooringTrigger mooringTrigger;

    [SerializeField]
    private List<Moorable> mooredShips;

    [SerializeField]
    private List<CrewMember> availableCrew;

    [SerializeField]
    private List<ShipForSale> shipsForSale;

    public MooringTrigger MooringTrigger
    {
        get { return mooringTrigger; }
    }

    public List<CrewMember> AvailableCrew
    {
        get
        {
            return availableCrew;
        }
        set
        {
            availableCrew = value == null ?
                 new List<CrewMember>() :
                 new List<CrewMember>(value);
        }
    }

    public List<ShipForSale> ShipsForSale
    {
        get
        {
            return shipsForSale;
        }
        set
        {
            shipsForSale = value == null ?
                new List<ShipForSale>() :
                new List<ShipForSale>(value);
        }
    }

    public void RequestMooring(Moorable moorable)
    {
        Debug.Assert(!mooredShips.Contains(moorable) && moorable.gameObject.activeInHierarchy,
            "tried to moor an already moored or disabled ship");

        moorable.gameObject.SendMessage("OnMoored", this, SendMessageOptions.DontRequireReceiver);

        moorable.gameObject.SetActive(false);
        mooredShips.Add(moorable);
    }

    public void Unmoor(Moorable moorable)
    {
        Debug.AssertFormat(mooredShips.Contains(moorable), 
            "tried to unmoor {0} from {1} but it's not moored here",
                moorable,
                gameObject);

        mooredShips.Remove(moorable);
            
        moorable.transform.position = mooringTrigger.transform.position;
        moorable.transform.rotation = mooringTrigger.transform.rotation;

        var rigidBody = moorable.GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
        }

        moorable.gameObject.SetActive(true);
        moorable.gameObject.SendMessage("OnUnmoored", this, SendMessageOptions.DontRequireReceiver);
    }
}
