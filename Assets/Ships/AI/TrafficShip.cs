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

    //our goal is to show up, visit the station, then leave
    [SerializeField]
    private bool hasVisitedStation;

    private AITaskFollower ai;

    private SpaceStation spaceStation;

    void Start()
    {
        ai = GetComponent<AITaskFollower>();
        hasVisitedStation = false;
    }
    
    private void Update()
    {
        if (ai.Idle)
        {
            if (spaceStation && !hasVisitedStation)
            {
                SetDestinationStation(spaceStation);
            }
            else
            {
                JumpOut();
            }
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
}
