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
    private DockingTrigger dockingTrigger;

    [SerializeField]
    private List<Transform> undockPoints;

    [SerializeField]
    private Transform dockingViewpoint;

    [Header("Inside")]

    [SerializeField]
    private List<DockableObject> dockedShips;

    [SerializeField]
    private List<TrafficShip> dockedTraffic;

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
        var dockable = activator.Dockable;
        Debug.Assert(dockable);

        dockable.BeginAutoDocking(this);
    }

    public override bool CanBeActivatedBy(Ship activator)
    {
        return activator.Dockable.LocalStation == this;
    }

    public DockingTrigger DockingTrigger
    {
        get { return dockingTrigger; }
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

    public IEnumerable<ShipForSale> ShipsForSale
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

    public IEnumerable<CrewMember> FindAvailableCrew()
    {
        return SpaceTraderConfig.CrewConfiguration.Characters
            .Where(c => c.AtStation == this);
    }

    //eat a ship, disabling it and storing it in the hangar
    public void AddDockedShip(DockableObject docked)
    {
        //just in case...
        docked.transform.position = transform.position;

        docked.gameObject.SendMessage("OnDocked", this, SendMessageOptions.DontRequireReceiver);

        docked.gameObject.SetActive(false);
        dockedShips.Add(docked);
        var traffic = docked.GetComponent<TrafficShip>();
        if (traffic)
        {
            dockedTraffic.Add(traffic);
        }
    }

    public void Undock(DockableObject dockable)
    {
        Debug.AssertFormat(dockedShips.Contains(dockable),
            "tried to undock {0} from {1} but it's not docked here",
                dockable,
                gameObject);

        dockedShips.Remove(dockable);
        dockedTraffic.RemoveAll(t => t.Ship.Dockable == dockable);

        Transform undockPoint;
        if (undockPoints.Count == 0)
        {
            undockPoint = dockingTrigger.transform;
        }
        else
        {
            undockPoint = undockPoints[UnityEngine.Random.Range(0, undockPoints.Count)];
        }

        dockable.transform.position = undockPoint.position;
        dockable.transform.rotation = undockPoint.rotation;

        var rigidBody = dockable.GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
        }

        var ship = dockable.GetComponent<Ship>();
        if (ship)
        {
            ship.ResetControls();
        }

        dockable.gameObject.SetActive(true);
        dockable.gameObject.SendMessage("OnUndocked", this, SendMessageOptions.DontRequireReceiver);
    }

    public void UndockPlayer()
    {
        /*have to run this as a routine on the station since both the player
         and the undock gui disable themselves during this process! */
        var player = SpaceTraderConfig.LocalPlayer;
        Debug.Assert(dockedShips.Contains(player.Ship.Dockable));

        Undock(player.Ship.Dockable);
    }

    void Update()
    {
        for (int trafficIt = 0; trafficIt < dockedTraffic.Count; ++trafficIt)
        {
            dockedTraffic[trafficIt].DockedUpdate(this);
        }
    }
}
