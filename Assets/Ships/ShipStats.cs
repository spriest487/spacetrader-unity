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
        agility = 0;
        thrust = 0;

        maxTurnSpeed = 0;
        maxSpeed = 0;
    }

    public ShipStats(ShipStats other)
    {
        agility = other.agility;
        thrust = other.thrust;

        maxTurnSpeed = other.maxTurnSpeed;
        maxSpeed = other.maxSpeed;
    }

    public void AddFlat(ShipStats other)
    {
        agility += other.agility;
        thrust += other.thrust;
        maxTurnSpeed += other.maxTurnSpeed;
        maxSpeed += other.maxSpeed;
    }

    public void ApplyProportional(ShipStats other)
    {
        agility += agility * other.agility;
        thrust += thrust * other.thrust;
        maxTurnSpeed += maxTurnSpeed * other.maxTurnSpeed;
        maxSpeed += maxSpeed * other.maxSpeed;
    }
}
