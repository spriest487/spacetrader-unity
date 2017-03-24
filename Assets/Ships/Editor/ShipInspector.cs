using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(Ship))]
public class ShipInspector : Editor
{
    private ModulePreset applyPreset;

    private bool showCrew = false;
    private bool showFleet = true;

    private void CrewField(CrewMember crewMember, CrewAssignment assignment)
    {
        var selected = EditorGUILayout.ObjectField(crewMember, typeof(CrewMember), true) as CrewMember;

        if (crewMember != selected)
        {
            if (crewMember)
            {
                crewMember.Unassign(null);
            }

            if (selected)
            {
                selected.Assign(target as Ship, assignment);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var ship = target as Ship;

        if (GUILayout.Button("Recalculate stats"))
        {
            ship.RecalculateCurrentStats();
        }

        if (GUILayout.Button("Reset cargo hold"))
        {
            ship.Cargo = CreateInstance<CargoHold>();
        }

        if (GUILayout.Button("Reset weapon hardpoints"))
        {
            for (int mod = 0; mod < ship.ModuleLoadout.SlotCount; ++mod)
            {
                ship.ModuleLoadout.RemoveAt(mod);
                ship.ModuleLoadout.Equip(mod, null);
            }
        }

        applyPreset = EditorGUILayout.ObjectField("Select module preset", applyPreset, typeof(ModulePreset), false) as ModulePreset;
        GUI.enabled = applyPreset;
        if (GUILayout.Button("Apply"))
        {
            applyPreset.Apply(ship);
            applyPreset = null;
        }
        GUI.enabled = true;

        if ((showCrew = EditorGUILayout.Foldout(showCrew, "Crew Assignments")))
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Captain");

                CrewField(ship.GetCaptain(), CrewAssignment.Captain);

                EditorGUILayout.LabelField("Passengers");
                var passengers = ship.GetPassengers().ToList();
                passengers.Resize((int)ship.CurrentStats.PassengerCapacity);

                foreach (var passenger in passengers)
                {
                    CrewField(passenger, CrewAssignment.Passenger);
                }
            }
            else
            {
                EditorGUILayout.LabelField("(crew assignment not available when not playing)");
            }
        }

        if ((showFleet = EditorGUILayout.Foldout(showFleet, "Fleet")))
        {
            Fleet fleet;
            if (Application.isPlaying && 
                (fleet = Universe.FleetManager.GetFleetOf(ship)))
            {
                if (GUILayout.Button("Select"))
                {
                    Selection.activeObject = fleet;
                }
            }
            else
            {
                EditorGUILayout.LabelField("(no fleet)");
            }
        }
    }
}