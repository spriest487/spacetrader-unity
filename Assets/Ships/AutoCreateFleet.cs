#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Ship))]
public class AutoCreateFleet : MonoBehaviour
{
    [SerializeField]
    private List<Ship> members;

    private void Start()
    {
        var leader = GetComponent<Ship>();

        members.ForEach(member => SpaceTraderConfig.FleetManager.AddToFleet(leader, member));

        Destroy(this);
    }
}
