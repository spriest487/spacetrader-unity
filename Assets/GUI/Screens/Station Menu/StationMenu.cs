using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StationMenu : MonoBehaviour
{
    [SerializeField]
    private string headerFormat = "Docked at {0}";

    [SerializeField]
    private Text headerText;

    private Moorable GetPlayerMoorable()
    {
        var player = PlayerShip.LocalPlayer;
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
        if (moorable && moorable.SpaceStation)
        {
            moorable.SpaceStation.Unmoor(moorable);
        }
    }

    void Start()
    {
    }

    void Update()
    {
        var moorable = GetPlayerMoorable();

        if (headerText)
        {
            headerText.text = string.Format(headerFormat, moorable.SpaceStation.name);
        }
    }
}
