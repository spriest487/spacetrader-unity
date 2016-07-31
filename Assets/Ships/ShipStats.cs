﻿using System;
using UnityEngine;

[Serializable]
public class ShipStats
{
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

    [SerializeField]
    private float armor;

    [SerializeField]
    private float shield;

    [SerializeField]
    private float mass;

    /// <summary>
    /// Number of passenger slots
    /// </summary>
    public uint PassengerCapacity { get { return (uint) Mathf.FloorToInt(passengerCapacity); } }
    public int Armor { get { return Mathf.FloorToInt(armor); } }
    public int Shield { get { return Mathf.FloorToInt(shield); } }
    public float Mass { get { return mass; } }

    public ShipStats()
    {
        agility = 0;
        thrust = 0;

        maxTurnSpeed = 0;
        maxSpeed = 0;

        passengerCapacity = 0;

        armor = 0;
        shield = 0;

        mass = 0;
    }

    public ShipStats Clone()
    {
        return (ShipStats) MemberwiseClone();
    }

    public void AddFlat(ShipStats other)
    {
        agility += other.agility;
        thrust += other.thrust;
        maxTurnSpeed += other.maxTurnSpeed;
        maxSpeed += other.maxSpeed;
        passengerCapacity += other.passengerCapacity;

        armor += other.armor;
        shield += other.shield;

        mass += other.mass;
    }

    public void ApplyProportional(ShipStats other)
    {
        agility += agility * other.agility;
        thrust += thrust * other.thrust;
        maxTurnSpeed += maxTurnSpeed * other.maxTurnSpeed;
        maxSpeed += maxSpeed * other.maxSpeed;
        passengerCapacity += passengerCapacity * other.passengerCapacity;

        armor += armor * other.armor;
        shield += shield * other.shield;

        mass += mass * other.mass;
    }
}
