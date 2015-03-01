using UnityEngine;
using System.Collections;

public class PlayerStart : MonoBehaviour {
    public static PlayerShip ActivePlayer
    {
        get;
        private set;
    }

    public PlayerShip playerPrefab;

    void OnEnable()
    {
        /* if this client doesn't have a player object, try to create one */
        if (!ActivePlayer && !(ActivePlayer = FindObjectOfType<PlayerShip>())) 
        {
            if (!playerPrefab)
            { 
                throw new UnityException("player start requires a player instance");
            }

            if (Network.isClient || Network.isServer)
            {
                ActivePlayer = (PlayerShip)Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
                Debug.Log("spawning network player " + Network.player.guid);
            }
            else
            {
                ActivePlayer = (PlayerShip)Instantiate(playerPrefab, transform.position, transform.rotation);
            }
        }
    }
}
