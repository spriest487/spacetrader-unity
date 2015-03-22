using UnityEngine;
using System.Collections;

public class PlayerStart : MonoBehaviour {
    public PlayerShip playerPrefab;
    
    void Start()
    {
        if (!PlayerShip.LocalPlayer)
        {
            var player = (PlayerShip)Instantiate(playerPrefab, transform.position, transform.rotation);
            player.MakeLocal();
        }
        else
        {
            Debug.LogWarning("playerstart tried to spawn a player, but there was already one active");
        }
    }
}
