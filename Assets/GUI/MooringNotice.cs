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
        var player = PlayerManager.Player;

        bool active = false;
        if (player)
        {
            var moorable = player.GetComponent<Moorable>();
            if (moorable)
            {
                if (moorable.spaceStation)
                {
                    text.text = string.Format(template, 
                        "ACTIVATE",
                        moorable.spaceStation.name);
                    active = true;
                }
            }
        }

        text.enabled = active;
    }
}
