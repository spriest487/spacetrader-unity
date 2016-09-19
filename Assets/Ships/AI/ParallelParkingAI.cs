using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ParallelParkingAI : MonoBehaviour
{
    public Transform[] flyTo;
    public Transform parkAt;

    private AITaskFollower ai;

    void Start()
    {
        ai = GetComponent<AITaskFollower>();

        var points = flyTo.ToList();
        points.Reverse();

        foreach (var point in points)
        {
            ai.AssignTask(NavigateTask.Create(point.position));
        }
    }

    void Update()
    {
        if (ai.Idle)
        {
            ai.Ship.ResetControls(0, 0, 0, 0, 0, 0);
            ai.Ship.RotateToPoint(parkAt.position + Vector3.forward * 100, parkAt.transform.up);
            GetComponent<Ship>().PreciseManeuverTo(parkAt.position);
        }
    }
}