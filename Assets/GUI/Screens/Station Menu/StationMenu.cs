using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StationMenu : MonoBehaviour
{
    public string headerFormat = "Docked at {0}";

    private Text headerText;

    private Moorable GetPlayerMoorable()
    {
        var player = PlayerStart.activePlayer;
        if (player)
        {
            return player.GetComponent<Moorable>();
        }
        else
        {
            return null;
        }
    }
        
    public void Undock()
    {
        var moorable = GetPlayerMoorable();
        if (moorable && moorable.spaceStation)
        {
            moorable.spaceStation.Unmoor(moorable);
        }
    }

    void Start()
    {
        var header = transform.Find("Header");
        if (header)
        {
            headerText = header.gameObject.GetComponent<Text>();
        }
    }

    void Update()
    {
        var moorable = GetPlayerMoorable();

        if (headerText)
        {
            headerText.text = string.Format(headerFormat, moorable.spaceStation.name);
        }
    }
}
