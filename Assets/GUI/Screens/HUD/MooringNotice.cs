using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class MooringNotice : MonoBehaviour
{
    public string template = "Press {0} to dock with {1}";

    private Text text;
    
    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        var player = PlayerShip.LocalPlayer;

        bool active = false;
        if (player)
        {
            var moorable = player.GetComponent<Moorable>();
            if (moorable && moorable.LocalStation && moorable.State == DockingState.InSpace)
            {
                text.text = string.Format(template, 
                    "ACTIVATE",
                    moorable.LocalStation.name);
                active = true;
            }
        }

        text.gameObject.SetActive(active);
    }
}
