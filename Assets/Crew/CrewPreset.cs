using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Ship))]
public class CrewPreset : MonoBehaviour
{
    [SerializeField]
    private List<CrewMember> passengers;

    [SerializeField]
    private CrewMember captain;

    void Start()
    {
        var ship = GetComponent<Ship>();
        var characters = SpaceTraderConfig.CrewConfiguration;

        passengers.Where(p => !!p)
            .Select(p => characters.NewCharacter(p))
            .ToList()
            .ForEach(p => p.Assign(ship, CrewAssignment.Passenger));

        if (captain)
        {
            characters.NewCharacter(captain).Assign(ship, CrewAssignment.Captain);
        }

        Destroy(this);
    }
}