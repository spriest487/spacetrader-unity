#pragma warning disable 0649

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

    [SerializeField]
    private Sprite portrait;

    public int PilotSkill { get { return pilotSkill; } }
    public int WeaponsSkill { get { return weaponsSkill; } }
    public Sprite Portrait { get { return portrait; } }

    public static CrewMember Create(string name, Sprite portrait)
    {
        CrewMember result = CreateInstance<CrewMember>();
        result.name = name;
        result.portrait = portrait;

        return result;
    }
}
