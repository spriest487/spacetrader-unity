using UnityEngine;

public class Targetable : MonoBehaviour
{
    [SerializeField]
	private string faction;

    [SerializeField]
    private bool hideBracket;

    private Ship ship;

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
                    && PlayerShip.LocalPlayer.gameObject == this.gameObject;

                return !isPlayer;
            }
        }
    }

    public string Faction { get { return faction; } set { faction = value; } }

    private void Start()
    {
        ship = GetComponent<Ship>();
    }

    public int RelationshipTo(Targetable other)
    {
        if (other
            || string.IsNullOrEmpty(other.Faction)
            || string.IsNullOrEmpty(Faction))
        {
            return 0;
        }

        bool sameFaction = Faction == other.Faction;
        return sameFaction ? 1 : -1;
    }
}