using UnityEngine;
using System.Collections;

public class PlayerStart : MonoBehaviour {
    public static PlayerShip activePlayer
    {
        get;
        private set;
    }

    public PlayerShip playerPrefab;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            if (!playerPrefab)
            {
                throw new UnityException("player start requires a player instance");
            }

            activePlayer = (PlayerShip)Instantiate(playerPrefab, transform.position, transform.rotation);

            Application.LoadLevelAdditive("WorldCommon");
        }
    }

    void OnWorldEnd()
    {
        activePlayer = null;
    }
}
