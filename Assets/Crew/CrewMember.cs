#pragma warning disable 0649

using System.Collections.Generic;
using System;
using UnityEngine;

public enum CrewAssignment
{
    Unassigned,
    Passenger,
    Captain,
}

public class CrewMember : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Crew/Crew member")]
    public static void CreateFromMenu()
    {
        ScriptableObjectUtility.CreateAsset<CrewMember>();
    }
#endif

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

    public void Assign(Ship ship, CrewAssignment role)
    {
        Debug.Assert(role != CrewAssignment.Unassigned);

        assignedShip = ship;
        assignedRole = role;
    }

    public void Unassign()
    {
        assignedRole = CrewAssignment.Unassigned;
        assignedShip = null;
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
