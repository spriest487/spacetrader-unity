#pragma warning disable 0649

using UnityEngine;
using System.Collections;

public class PlayerStart : MonoBehaviour {
    [SerializeField]
    private Ship playerPrefab;
    
    void Start()
    {
        if (Universe.LocalPlayer)
        {
            Debug.LogError("there's already a player spawned");
        }
        else
        {
            var playerObj = (Ship)Instantiate(playerPrefab, transform.position, transform.rotation);
            var player = playerObj.gameObject.AddComponent<PlayerShip>();

            Universe.LocalPlayer = player;
        }
    }
}
