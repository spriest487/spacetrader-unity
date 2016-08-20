using UnityEngine;
using System.Collections;
using System.Linq;

public enum DockingState
{
    InSpace,
    AutoDocking,
    Docked,
}

[RequireComponent(typeof(Ship))]
public class Moorable : MonoBehaviour
{
    [SerializeField]
    private DockingState state;

    [SerializeField]
    private SpaceStation localStation;

    private Ship ship;

    public DockingState State { get { return state; } }
    public SpaceStation LocalStation
    {
        get
        {
            return state == DockingState.InSpace ? localStation : null;
        }
    }

    public SpaceStation AutoDockingStation
    {
        get
        {
            return state == DockingState.AutoDocking ? localStation : null;
        }
    }

    public SpaceStation DockedAtStation
    {
        get
        {
            return state == DockingState.Docked ? localStation : null;
        }
    }
    
    void OnTriggerEnter(Collider collider)
    {
        if (State == DockingState.AutoDocking)
        {
            return;
        }

        var mooringTrigger = collider.GetComponent<MooringTrigger>();
        if (mooringTrigger)
        {
            if (localStation)
            {
                Debug.LogWarning("triggered multiple spacestation mooring points, ignoring " + collider);
            }
            else
            {
                localStation = mooringTrigger.SpaceStation;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (State == DockingState.AutoDocking)
        {
            return;
        }

        if (localStation && localStation.MooringTrigger.Collider == collider)
        {
            localStation = null;
        }
    }

    void Start()
    {
        ship = GetComponent<Ship>();
        localStation = null;
        state = DockingState.InSpace;
    }

    void OnMoored(SpaceStation station)
    {
        localStation = station;
        state = DockingState.Docked;
    }

    void OnUnmoored(SpaceStation station)
    {
        BeginAutoUndocking(station);
    }

    //TODO
    const float DOCK_DISTANCE = 25;

    private float GetDockProximity()
    {
        //TODO
        return 5;
    }

    private IEnumerator AutoDockingRoutine(SpaceStation spaceStation)
    {
        var points = spaceStation.UndockPoints.ToList();
        var pointIndex = UnityEngine.Random.Range(0, points.Count);

        var endPoint = points[pointIndex];
        var startPoint = points[pointIndex] + (endPoint - spaceStation.transform.position).normalized * DOCK_DISTANCE; //TODO

        var proximity = GetDockProximity();

        //TODO
        var shipAi = ship.GetComponent<AITaskFollower>();
        if (!shipAi)
        {
            shipAi = ship.gameObject.AddComponent<AITaskFollower>();
            yield return null; //Start() needs to run
        }

        var goToEnd = FlyToPointTask.Create(endPoint, proximity);

        shipAi.AssignTask(goToEnd);

        if (!ship.CanSee(endPoint) && !goToEnd.Done)
        {
            var goToStart = FlyToPointTask.Create(startPoint, proximity);
            shipAi.AssignTask(goToStart);

            while (!goToStart.Done)
            {
                yield return null;
            }
        }
        
        while (!goToEnd.Done)
        {
            yield return null;
        }

        state = DockingState.Docked;
        spaceStation.AddDockedShip(this);
        
        shipAi.ClearTasks();
    }

    public void BeginAutoDocking(SpaceStation spaceStation)
    {
        state = DockingState.AutoDocking;
        localStation = spaceStation;

        StartCoroutine(AutoDockingRoutine(spaceStation));
    }

    private IEnumerator AutoUndockingRoutine(SpaceStation station)
    {
        //station should undock us in a safe place pointing the right way
        var dest = transform.position + transform.forward * DOCK_DISTANCE;
        var proximity = GetDockProximity();

        while ((ship.transform.position - dest).sqrMagnitude > proximity)
        {
            ship.ResetControls(thrust: 1);

            yield return null;
        }

        state = DockingState.InSpace;
        localStation = null;

        var shipAi = ship.GetComponent<AITaskFollower>();
        if (shipAi)
        {
            shipAi.ClearTasks();
        }
    }

    private void BeginAutoUndocking(SpaceStation station)
    {
        localStation = station;
        state = DockingState.AutoDocking;

        StartCoroutine(AutoUndockingRoutine(station));
    }
}
