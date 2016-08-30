using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AITaskFollower))]
public class TrafficShip : MonoBehaviour
{
    private AITaskFollower ai;

    void Start()
    {
        ai = GetComponent<AITaskFollower>();
    }

    void Update()
    {
        if (!ai.Idle)
        {
            return;
        }


    }


}
