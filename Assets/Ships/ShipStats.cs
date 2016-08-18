using System;
using UnityEngine;

[Serializable]
public class ShipStats
{
    /// <summary>
    /// Turn acceleration, in degrees/sec^2
    /// </summary>
	[SerializeField]
    private float agility;

    /// <summary>
    /// Main engine acceleration, in m/s^2
    /// </summary>
	[SerializeField]
    private float thrust;

    /// <summary>
    /// Turn speed cap, in degrees/sec
    /// </summary>
	[SerializeField]
    private float maxTurnSpeed;

    /// <summary>
    /// Main engine speed cap, in m/s
    /// </summary>
    [SerializeField]
	private float maxSpeed;

    [SerializeField]
    private float passengerCapacity;

    [SerializeField]
    private float armor;

    [SerializeField]
    private float shield;

    [SerializeField]
    private float mass;

    [SerializeField]
    private float damageMultiplier;

    /// <summary>
    /// Number of passenger slots
    /// </summary>
    public uint PassengerCapacity
    {
        get { return (uint) Mathf.FloorToInt(passengerCapacity); }
    }

    public int Armor
    {
        get { return Mathf.FloorToInt(armor); }
    }

    public int Shield
    {
        get { return Mathf.FloorToInt(shield); }
    }

    public float PassengerCapacityRaw
    {
        get { return passengerCapacity; }
        set { passengerCapacity = value; }
    }

    public float ArmorRaw
    {
        get { return armor; }
        set { armor = value; }
    }

    public float ShieldRaw
    {
        get { return shield; }
        set { shield = value; }
    }

    public float Mass
    {
        get { return mass; }
        set { mass = value; }
    }

    public float Agility
    {
        get { return agility; }
        set { agility = value; }
    }

    public float Thrust
    {
        get { return thrust; }
        set { thrust = value; }
    }

    public float MaxTurnSpeed
    {
        get { return maxTurnSpeed; }
        set { maxTurnSpeed = value; }
    }

    public float MaxSpeed
    {
        get { return maxSpeed; }
        set { maxSpeed = value; }
    }

    public float DamageMultiplier
    {
        get { return damageMultiplier; }
        set { damageMultiplier = value; }
    }

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

        damageMultiplier = 0;
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

        damageMultiplier += other.damageMultiplier;
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

        damageMultiplier += mass * other.damageMultiplier;
    }
}
