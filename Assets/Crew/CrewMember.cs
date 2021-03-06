﻿#pragma warning disable 0649

using UnityEngine;

public enum CrewAssignment
{
    Unassigned,
    Passenger,
    Captain,
}

[CreateAssetMenu(menuName = "SpaceTrader/Crew/Crew Member")]
public class CrewMember : ScriptableObject
{
    [SerializeField]
    private Sprite portrait;

    [Header("Skill Points")]
    [SerializeField]
    private int pilotSkill;

    [SerializeField]
    private int weaponsSkill;

    [SerializeField]
    private int mechanicalSkill;

    [SerializeField]
    private int xp;

    [Header("Crew Assignment")]

    [SerializeField]
    private Ship assignedShip;

    [SerializeField]
    private SpaceStation atStation;

    [SerializeField]
    private CrewAssignment assignedRole;

    public int PilotSkill
    {
        get { return pilotSkill; }
        set { pilotSkill = value; }
    }

    public int WeaponsSkill
    {
        get { return weaponsSkill; }
        set { weaponsSkill = value; }
    }

    public int MechanicalSkill
    {
        get { return mechanicalSkill; }
        set { mechanicalSkill = value; }
    }

    public int Level
    {
        get { return pilotSkill + weaponsSkill + mechanicalSkill; }
    }

    public Sprite Portrait
    {
        get { return portrait; }
        set { portrait = value; }
    }

    public Ship AssignedShip
    {
        get { return assignedShip; }
    }

    public CrewAssignment AssignedRole
    {
        get { return assignedRole; }
    }

    public int XP
    {
        get { return xp; }
    }

    public SpaceStation AtStation { get { return atStation; } }

    public static CrewMember CreateRandom()
    {
        var character = ScriptableObject.CreateInstance<CrewMember>();
        character.RandomStats(10);
        return character;
    }

    public void Assign(Ship ship, CrewAssignment role)
    {
        Debug.Assert(role != CrewAssignment.Unassigned, "can't assign a crew member to the Unassigned role");

        assignedShip = ship;
        assignedRole = role;
        atStation = null;
    }

    public void Unassign(SpaceStation station)
    {
        assignedRole = CrewAssignment.Unassigned;
        assignedShip = null;
        atStation = station;
    }

    public void RandomizeStats()
    {
        RandomStats(PilotSkill + MechanicalSkill + WeaponsSkill);
    }

    public void RandomStats(int budget)
    {
        var weights = new float[3];
        float weightSum = 0;
        for (int skill = 0; skill < 3; ++skill)
        {
            var weight = UnityEngine.Random.Range(0, 1.0f);
            weightSum += weight;
            weights[skill] = weight;
        }

        for (int skill = 0; skill < 3; ++skill)
        {
            weights[skill] /= weightSum;
        }

        var skills = new int[3];
        int skillTotal = 0;
        for (int skill = 0; skill < 3; ++skill)
        {
            var points = Mathf.FloorToInt(weights[skill] * budget);
            skillTotal += points;
            skills[skill] = points;
        }

        if (skillTotal < budget)
        {
            skills[UnityEngine.Random.Range(0, 3)] += budget - skillTotal;
        }

        PilotSkill = skills[0];
        WeaponsSkill = skills[1];
        MechanicalSkill = skills[2];
    }

    public void GrantXP(int amount)
    {
        Debug.Assert(amount >= 0, "xp should never go down");

        xp += amount;

        if (assignedShip)
        {
            var gain = new XPGain()
            {
                Amount = amount,
                CrewMember = this
            };

            assignedShip.SendMessage("OnCrewMemberGainedXP", gain, SendMessageOptions.DontRequireReceiver);
        }
    }
}
