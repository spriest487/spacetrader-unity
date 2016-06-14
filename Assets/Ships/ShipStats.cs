using System;
using UnityEngine;

[Serializable]
public class ShipStats
{
    public float estimatedDps;

    /// <summary>
    /// Turn acceleration, in degrees/sec^2
    /// </summary>
	public float agility;

    /// <summary>
    /// Main engine acceleration, in m/s^2
    /// </summary>
	public float thrust;

    /// <summary>
    /// Turn speed cap, in degrees/sec
    /// </summary>
	public float maxTurnSpeed;

    /// <summary>
    /// Main engine speed cap, in m/s
    /// </summary>
	public float maxSpeed;

    [SerializeField]
    private float passengerCapacity;

    /// <summary>
    /// Number of passenger slots
    /// </summary>
    public uint PassengerCapacity
    {
        get { return (uint) Mathf.FloorToInt(passengerCapacity); }
    }

    public ShipStats()
    {
        estimatedDps = 0;

        agility = 0;
        thrust = 0;

        maxTurnSpeed = 0;
        maxSpeed = 0;

        passengerCapacity = 0;
    }

    public ShipStats(ShipStats other)
    {
        estimatedDps = other.estimatedDps;

        agility = other.agility;
        thrust = other.thrust;

        maxTurnSpeed = other.maxTurnSpeed;
        maxSpeed = other.maxSpeed;

        passengerCapacity = other.PassengerCapacity;
    }

    public void AddFlat(ShipStats other)
    {
        estimatedDps += other.estimatedDps;
        agility += other.agility;
        thrust += other.thrust;
        maxTurnSpeed += other.maxTurnSpeed;
        maxSpeed += other.maxSpeed;
        passengerCapacity += other.passengerCapacity;
    }

    public void ApplyProportional(ShipStats other)
    {
        other.estimatedDps += agility * other.estimatedDps;
        agility += agility * other.agility;
        thrust += thrust * other.thrust;
        maxTurnSpeed += maxTurnSpeed * other.maxTurnSpeed;
        maxSpeed += maxSpeed * other.maxSpeed;
        passengerCapacity += passengerCapacity * other.passengerCapacity;
    }
}
