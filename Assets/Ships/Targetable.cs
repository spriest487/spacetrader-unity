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
                var isPlayer = PlayerStart.ActivePlayer
                    && PlayerStart.ActivePlayer.gameObject == this.gameObject;

                return !isPlayer;
            }
        }
    }

    public string Faction { get { return faction; } }
}