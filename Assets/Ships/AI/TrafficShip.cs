using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AITaskFollower))]
public class TrafficShip : MonoBehaviour
{    
    private AITaskFollower ai;

    private SpaceStation spaceStation;

    void Start()
    {
        ai = GetComponent<AITaskFollower>();
    }
    
    private void Update()
    {
        if (ai.Idle)
        {
            JumpOut();
        }
    }

    public void SetDestinationStation(SpaceStation station)
    {
        spaceStation = station;
    }

    void OnTakeDamage(HitDamage damage)
    {
        ai.ClearTasks();
    }
    
    void FlyToStation()
    {
        ai.AssignTask(FlyToPointTask.Create(spaceStation.transform.position, ai.Ship.CloseDistance));
    }

    void JumpOut()
    {
        //pick a random world map point
        var mapPoint = SpaceTraderConfig.WorldMap.DistantAreas.ToList().Random();

        const float JUMP_OUT_CLEAR_DIST = 50;
        var targetPos = mapPoint.transform.localPosition * JUMP_OUT_CLEAR_DIST;

        ai.AssignTask(FlyToPointTask.Create(targetPos, ai.Ship.CloseDistance));
    }
}
