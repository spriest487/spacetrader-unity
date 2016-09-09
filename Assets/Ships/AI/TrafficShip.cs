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
            //maybe a task for this instead
            if (spaceStation && ai.Ship.IsCloseTo(spaceStation.transform.position))
            {
                spaceStation.Activate(ai.Ship);
            }

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
        ai.AssignTask(ActivateTask.Create(spaceStation));
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
