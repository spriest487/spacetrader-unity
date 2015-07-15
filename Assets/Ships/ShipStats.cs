using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ShipStats
{
	public float agility;
	public float thrust;

	public float maxTurnSpeed;
	public float maxSpeed;

    public ShipStats()
    {
        agility = 1;
        thrust = 1;

        maxTurnSpeed = 1;
        maxSpeed = 1;
    }

    public ShipStats(ShipStats other)
    {
        agility = other.agility;
        thrust = other.thrust;

        maxTurnSpeed = other.maxTurnSpeed;
        maxSpeed = other.maxSpeed;
    }
}
