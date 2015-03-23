using UnityEngine;

public class Targetable : MonoBehaviour
{
    [SerializeField]
	private string faction;

    [SerializeField]
    private bool hideBracket;

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
}