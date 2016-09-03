using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    [SerializeField]
    private ShipType shipType;

    [Header("Player Spawn")]

    [SerializeField]
    private bool makeLocalPlayer = false;

    [SerializeField]
    private int money = 1000;
    
    private void Start()
    {
        var ship = shipType.CreateShip(transform.position, transform.rotation);

        if (makeLocalPlayer)
        {
            Debug.Assert(!SpaceTraderConfig.LocalPlayer, "local player should not already be spawned");

            var player = ship.gameObject.AddComponent<PlayerShip>();
            player.AddMoney(money);

            SpaceTraderConfig.LocalPlayer = player;
        }

        Destroy(gameObject);
    }
}