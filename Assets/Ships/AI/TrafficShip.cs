using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AITaskFollower))]
public class TrafficShip : MonoBehaviour
{
    public static TrafficShip AddToShip(Ship ship)
    {
        var ai = ship.GetComponent<AITaskFollower>();
        if (!ai)
        {
            ai = AITaskFollower.AddToShip(ship);
        }

        var trafficShip = ship.gameObject.AddComponent<TrafficShip>();
        trafficShip.Start();

        return trafficShip;
    }

    //our goal is to show up, visit the station, then leave - once this != 0 we're going home
    [SerializeField]
    private float dockedTime;

    private AITaskFollower ai;

    private SpaceStation spaceStation;

    public Ship Ship { get { return ai.Ship; } }

    void Start()
    {
        ai = GetComponent<AITaskFollower>();
        dockedTime = 0;
    }
    
    private void Update()
    {
        if (ai.Idle)
        {
            if (spaceStation && dockedTime == 0)
            {
                SetDestinationStation(spaceStation);
            }
            else
            {
                //wait for dock/undock routines to finish before doing this
                if (Ship.Moorable.State == DockingState.InSpace)
                {
                    JumpOut();
                }
            }
        }
    }

    public void DockedUpdate(SpaceStation dockedAtStation)
    {
        Debug.Assert(dockedAtStation && ai.Ship && ai.Ship.Moorable);

        if (Time.time > dockedTime + 5)
        {
            dockedAtStation.Unmoor(ai.Ship.Moorable);

            //just to make sure..
            ai.ClearTasks();
        }
    }

    public void SetDestinationStation(SpaceStation station)
    {
        spaceStation = station;

        ai.AssignTask(ActivateTask.Create(spaceStation));

        //use the undock points to find our docking vector
        var undock = ai.transform.Closest(spaceStation.UndockPoints);
        if (undock)
        {
            ai.AssignTask(NavigateTask.Create(undock.transform.position + undock.transform.forward * Moorable.DOCK_DISTANCE));
        }
    }

    void OnTakeDamage(HitDamage damage)
    {
        ai.ClearTasks();
    }
    
    void JumpOut()
    {
        //pick a random world map point
        var mapPoint = SpaceTraderConfig.WorldMap.DistantAreas.ToList().Random();

        const float JUMP_OUT_CLEAR_DIST = 50;
        var targetPos = mapPoint.transform.localPosition * JUMP_OUT_CLEAR_DIST;

        ai.AssignTask(ActivateTask.Create(mapPoint));
    }

    void OnCompletedJump()
    {
        Destroy(gameObject);
    }

    void OnMoored()
    {
        dockedTime = Time.time;
    }
}
