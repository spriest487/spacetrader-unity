using UnityEngine;
using System.Collections.Generic;

public class TrafficSpawner : MonoBehaviour
{
    const float RANDOM_SPAWN_DIST = 100;

    /* make this a number high enough to not matter normally, but low enough
     to stop things getting silly when debugging with short spawn timers */
    const float MAX_SPAWNED_COUNT = 5;

    [SerializeField]
    private float spawnRateMin = 2;

    [SerializeField]
    private float spawnRateMax = 10;

    [SerializeField]
    private List<ShipType> spawnableTypes;

    private float nextSpawn;
    private SpaceStation station;
    
    private List<Ship> spawned;

    void Start()
    {
        station = GetComponent<SpaceStation>();
        spawned = new List<Ship>();

        RandomSpawnTimer();
    }

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            spawned.RemoveAll(s => !s);
            if (spawned.Count < MAX_SPAWNED_COUNT)
            {
                SpawnTrafficShip();
            }
            else
            {
                Debug.Log("not spawning a new traffic ship, already have too many");
            }

            RandomSpawnTimer();
        }
    }
   
    void SpawnTrafficShip()
    {
        Debug.Assert(spawnableTypes.Count > 0);

        var shipType = spawnableTypes.Random();
        
        var spawnPos = transform.position + Random.onUnitSphere * RANDOM_SPAWN_DIST;
        var spawnRot = Quaternion.LookRotation((transform.position - spawnPos).normalized);

        var ship = shipType.CreateShip(spawnPos, spawnRot);
        var trafficShip = TrafficShip.AddToShip(ship);
        trafficShip.SetDestinationStation(station);
        
        spawned.Add(ship);
    }

    void RandomSpawnTimer()
    {
        nextSpawn = Time.time + Random.Range(spawnRateMin, spawnRateMax);
    }
}
