#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class SpaceStation : ActionOnActivate
{
    [Header("Outside")]

    [SerializeField]
    private MooringTrigger mooringTrigger;

    [SerializeField]
    private List<Transform> undockPoints;

    [SerializeField]
    private Transform dockingViewpoint;

    [Header("Inside")]

    [SerializeField]
    private List<Moorable> dockedShips;

    [SerializeField]
    private List<TrafficShip> dockedTraffic;

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
        dockedShips.Add(docked);
        var traffic = docked.GetComponent<TrafficShip>();
        if (traffic)
        {
            dockedTraffic.Add(traffic);
        }
    }

    public void Unmoor(Moorable moorable)
    {
        Debug.AssertFormat(dockedShips.Contains(moorable),
            "tried to unmoor {0} from {1} but it's not moored here",
                moorable,
                gameObject);

        dockedShips.Remove(moorable);
        dockedTraffic.RemoveAll(t => t.Ship.Moorable == moorable);

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

    public void UndockPlayer()
    {
        /*have to run this as a routine on the station since both the player
         and the undock gui disable themselves during this process! */
        var player = SpaceTraderConfig.LocalPlayer;
        Debug.Assert(dockedShips.Contains(player.Ship.Moorable));

        Unmoor(player.Ship.Moorable);
    }

    void Update()
    {
        for (int trafficIt = 0; trafficIt < dockedTraffic.Count; ++trafficIt)
        {
            dockedTraffic[trafficIt].DockedUpdate(this);
        }
    }
}
