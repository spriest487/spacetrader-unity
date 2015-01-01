using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;

    public static GameObject Player
    {
        get
        {
            var cfg = GameObject.Find("World Config");
            var manager = cfg.GetComponent<PlayerManager>();
            return manager.player;
        }
    }
}
