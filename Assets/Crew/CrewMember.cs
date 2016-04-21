using System.Collections.Generic;
using System;
using UnityEngine;

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

    public int PilotSkill { get { return pilotSkill; } }
    public int WeaponsSkill { get { return weaponsSkill; } }

    public static CrewMember Create(string name)
    {
        CrewMember result = CreateInstance<CrewMember>();
        result.name = name;

        return result;
    }
}
