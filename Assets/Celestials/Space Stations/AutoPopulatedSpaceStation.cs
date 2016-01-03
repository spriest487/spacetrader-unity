using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SpaceStation))]
public class AutoPopulatedSpaceStation : MonoBehaviour
{
    private SpaceStation station;

    public void Start()
    {
        station = GetComponent<SpaceStation>();
    }

    public void Populate()
    {
        var crewCount = UnityEngine.Random.Range(0, 5);
        var newCrew = new List<CrewMember>(crewCount);
        
        for (int crewNo = 0; crewNo < crewCount; ++crewNo)
        {
            var member = CrewMember.Create("No name");
            newCrew.Add(member);
        }

        station.AvailableCrew = newCrew;
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(AutoPopulatedSpaceStation))]
public class AutoPopulatedSpaceStationInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var station = (AutoPopulatedSpaceStation)target;

        if (GUILayout.Button("Populate now"))
        {
            station.Populate();
        }
    }
}
#endif
