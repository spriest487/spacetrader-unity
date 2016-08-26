#pragma warning disable 0649

using UnityEngine;

public enum TargetRelationship
{
    Neutral,
    Hostile,
    Friendly,
    FleetMember,
    Resource
}

public class Targetable : MonoBehaviour
{
    [SerializeField]
	private string faction;

    [SerializeField]
    private bool hideBracket;
    
    private Ship ship;
    private ActionOnActivate actionOnActivate;

    public bool BracketVisible
    {
        get
        {
            if (hideBracket)
            {
                return false;
            }
            else
            {
                var isPlayer = PlayerShip.LocalPlayer
                    && PlayerShip.LocalPlayer.Ship == ship;

                return !isPlayer;
            }
        }
    }

    public ActionOnActivate ActionOnActivate { get { return actionOnActivate; } }
    
    public string Faction { get { return faction; } set { faction = value; } }

    private void Start()
    {
        ship = GetComponent<Ship>();
        actionOnActivate = GetComponent<ActionOnActivate>();
    }

    public TargetRelationship RelationshipTo(Targetable other)
    {
        if (ship && other.ship && ship.IsFleetMember(other.ship))
        {
            return TargetRelationship.FleetMember;
        }

        if (other.faction == "resource")
        {
            return TargetRelationship.Resource;
        }

        if (string.IsNullOrEmpty(other.Faction)
            || string.IsNullOrEmpty(Faction))
        {
            return TargetRelationship.Neutral;
        }
        
        bool sameFaction = Faction == other.Faction;
        return sameFaction ? TargetRelationship.Friendly : TargetRelationship.Hostile;
    }
}