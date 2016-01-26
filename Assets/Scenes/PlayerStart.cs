using UnityEngine;
using System.Collections;

public class PlayerStart : MonoBehaviour {
    [SerializeField]
    private Ship playerPrefab;
    
    void Start()
    {
        if (PlayerShip.LocalPlayer)
        {
            Debug.LogError("there's already a player spawned");
        }
        else
        {
            var playerObj = (Ship)Instantiate(playerPrefab, transform.position, transform.rotation);
            var player = playerObj.gameObject.AddComponent<PlayerShip>();

            SpaceTraderConfig.LocalPlayer = player;
        }
    }
}
