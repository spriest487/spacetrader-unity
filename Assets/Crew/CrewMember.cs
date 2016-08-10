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
    private int pilotSkill;

    [SerializeField]
    private int weaponsSkill;

    [SerializeField]
    private int mechanicalSkill;

    [SerializeField]
    private Sprite portrait;

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

#if UNITY_EDITOR
    private void OnDestroy()
    {
        Debug.Log("destroying crew member " +name);
    }
#endif
}
