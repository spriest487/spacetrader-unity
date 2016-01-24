using System;
using UnityEngine;

[Serializable]
public class ShipStats
{
	public float agility;
	public float thrust;

	public float maxTurnSpeed;
	public float maxSpeed;

    [SerializeField]
    private float passengerCapacity;

    public uint PassengerCapacity
    {
        get { return (uint) Mathf.FloorToInt(passengerCapacity); }
    }

    public ShipStats()
    {
        agility = 0;
        thrust = 0;

        maxTurnSpeed = 0;
        maxSpeed = 0;

        passengerCapacity = 0;
    }

    public ShipStats(ShipStats other)
    {
        agility = other.agility;
        thrust = other.thrust;

        maxTurnSpeed = other.maxTurnSpeed;
        maxSpeed = other.maxSpeed;

        passengerCapacity = other.PassengerCapacity;
    }

    public void AddFlat(ShipStats other)
    {
        agility += other.agility;
        thrust += other.thrust;
        maxTurnSpeed += other.maxTurnSpeed;
        maxSpeed += other.maxSpeed;
        passengerCapacity += other.passengerCapacity;
    }

    public void ApplyProportional(ShipStats other)
    {
        agility += agility * other.agility;
        thrust += thrust * other.thrust;
        maxTurnSpeed += maxTurnSpeed * other.maxTurnSpeed;
        maxSpeed += maxSpeed * other.maxSpeed;
        passengerCapacity += passengerCapacity * other.passengerCapacity;
    }
}
