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
        if (!(ActivePlayer = FindObjectOfType<PlayerShip>())) 
        {
            if (!playerPrefab)
            {
                throw new UnityException("player start requires a player instance");
            }

            ActivePlayer = (PlayerShip)Instantiate(playerPrefab, transform.position, transform.rotation);
        }
    }

    void OnLevelLoaded()
    {
        if (GameObject.Find("WorldCommonMarker") == null)
        {
            Application.LoadLevelAdditive("WorldCommon");
        }
    }
}
