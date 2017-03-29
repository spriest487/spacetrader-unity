using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Serializable]
    public class SpawnGroup
    {
        [SerializeField]
        private ShipSpawner[] spawners;
        
        public bool Spawned { get; private set; }
        
        public bool Alive
        {
            get { return spawners.All(s => !s || s.Spawned); }
        }

        public void SpawnAll()
        {
            foreach (var spawner in spawners.Where(s => s))
            {
                spawner.Spawn();
            }

            Spawned = true;
        }
    }
    
    [SerializeField]
    private SpawnGroup[] groups;

    [SerializeField]
    private float groupDelay = 1;

    private Coroutine spawnRoutine;
    
    private void OnEnable()
    {
        if (spawnRoutine == null)
        {
            StartCoroutine(SpawnGroups());
        }
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnGroups()
    {
        foreach (var group in groups)
        {
            if (group.Spawned)
            {
                continue;
            }

            yield return new WaitForSeconds(groupDelay);

            group.SpawnAll();
            while (group.Alive)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
