#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using System;

public class SpaceStation : ActionOnActivate
{
    [Header("Outside")]

    [SerializeField]
    private MooringTrigger mooringTrigger;

    [SerializeField]
    private List<Moorable> mooredShips;

    [SerializeField]
    private List<Transform> undockPoints;

    [SerializeField]
    private Transform dockingViewpoint;

    [Header("Inside")]

    [SerializeField]
    private List<CrewMember> availableCrew;

    [SerializeField]
    private List<ShipForSale> shipsForSale;

    [SerializeField]
    private CargoHold itemsForSale;

    public override string ActionName
    {
        get { return "ENTER STATION"; }
    }

    public override void Activate(Ship activator)
    {
        var moorable = activator.Moorable;
        Debug.Assert(moorable);

        moorable.BeginAutoDocking(this);
    }

    public override bool CanBeActivatedBy(Ship activator)
    {
        return activator.Moorable.LocalStation == this;
    }

    public MooringTrigger MooringTrigger
    {
        get { return mooringTrigger; }
    }
    
    public IEnumerable<Transform> UndockPoints
    {
        get { return undockPoints; }
    }

    public Vector3 DockingViewpoint
    {
        get { return dockingViewpoint.position; }
    }

    public CargoHold ItemsForSale
    {
        get
        {
            if (!itemsForSale)
            {
                itemsForSale = ScriptableObject.CreateInstance<CargoHold>();
                itemsForSale.Size = 1000;
            }

            return itemsForSale;
        }
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
            
    //eat a ship, disabling it and storing it in the hangar
    public void AddDockedShip(Moorable docked)
    {
        //just in case...
        docked.transform.position = transform.position;

        docked.gameObject.SendMessage("OnMoored", this, SendMessageOptions.DontRequireReceiver);

        docked.gameObject.SetActive(false);
        mooredShips.Add(docked);
    }

    public void Unmoor(Moorable moorable)
    {
        Debug.AssertFormat(mooredShips.Contains(moorable), 
            "tried to unmoor {0} from {1} but it's not moored here",
                moorable,
                gameObject);

        mooredShips.Remove(moorable);

        Transform undockPoint;
        if (undockPoints.Count == 0)
        {
            undockPoint = mooringTrigger.transform;
        }
        else
        {
            undockPoint = undockPoints[UnityEngine.Random.Range(0, undockPoints.Count)];
        }

        moorable.transform.position = undockPoint.position;
        moorable.transform.rotation = undockPoint.rotation;

        var rigidBody = moorable.GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
        }

        var ship = moorable.GetComponent<Ship>();
        if (ship)
        {
            ship.ResetControls();
        }

        moorable.gameObject.SetActive(true);
        moorable.gameObject.SendMessage("OnUnmoored", this, SendMessageOptions.DontRequireReceiver);
    }
}
