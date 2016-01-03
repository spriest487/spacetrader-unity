using System.Collections.Generic;
using System;
using UnityEngine;

public class CrewMember : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Crew Member")]
    public static void CreateFromMenu()
    {
        ScriptableObjectUtility.CreateAsset<CrewMember>();
    }
#endif

    public static CrewMember Create(string name)
    {
        CrewMember result = CreateInstance<CrewMember>();
        result.name = name;

        return result;
    }
}
