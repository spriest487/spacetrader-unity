using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(Ship))]
public class ShipInspector : Editor
{
    private ModulePreset applyPreset;

    private void CrewField(CrewMember crewMember, CrewAssignment assignment)
    {
        var selected = EditorGUILayout.ObjectField(crewMember, typeof(CrewMember), true) as CrewMember;

        if (crewMember != selected)
        {
            if (crewMember)
            {
                crewMember.Unassign();
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

        EditorGUILayout.LabelField("Crew assignments", EditorStyles.boldLabel);
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

        Hitpoints hp;
        if (Application.isPlaying && (hp = ship.GetComponent<Hitpoints>()))
        {
            if (GUILayout.Button("Destroy"))
            {
                hp.TakeDamage(1000000);
            }
        }
    }
}