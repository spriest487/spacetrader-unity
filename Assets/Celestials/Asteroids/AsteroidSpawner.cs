#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    struct SpawnPoint
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Asteroid Prefab;
    }

    [SerializeField]
    private int seed;

    [SerializeField]
    private Asteroid[] prefabs = new Asteroid[0];

    [SerializeField]
    private int count = 10;

    [SerializeField]
    private AnimationCurve scale = new AnimationCurve(new[]
        {
            new Keyframe(0, 1f),
            new Keyframe(1, 5.0f)
        });

    [SerializeField]
    private float scaleMultiplier = 1;

    [SerializeField]
    private float distance;
    
    [SerializeField]
    private float randomSpacing;

    private SpawnPoint[] GenerateSpawns()
    {
        var randState = Random.state;

        try
        {
            Random.InitState(seed);

            var presentPrefabs = prefabs.Where(p => p).ToList();
            if (presentPrefabs.Count == 0)
            {
                return new SpawnPoint[0];
            }

            var points = new SpawnPoint[count];
            for (int point = 0; point < points.Length; ++point)
            {
                var orbitPos = (360f / points.Length) * point;
                orbitPos += Random.Range(-randomSpacing, randomSpacing);

                var orbitRotation = Quaternion.Euler(0, orbitPos, 0);
                var randomPos = Matrix4x4.TRS(Vector3.zero, orbitRotation, Vector3.one)
                    .MultiplyPoint(Vector3.forward * distance);

                var randomScale = scaleMultiplier * scale.Evaluate(Random.Range(0f, 1f));

                points[point] = new SpawnPoint
                {
                    Position = randomPos,
                    Rotation = Quaternion.Euler(Random.Range(0f, 360f),
                        Random.Range(0f, 360f),
                        Random.Range(0f, 360f)),
                    Scale = new Vector3(randomScale, randomScale, randomScale),
                    Prefab = presentPrefabs[Random.Range(0, presentPrefabs.Count)],
                };
            }

            return points;
        }
        finally
        {
            Random.state = randState;
        }
    }

    private void Start()
    {
        foreach (var spawn in GenerateSpawns())
        {
            var asteroid = Instantiate(spawn.Prefab, 
                transform.position + spawn.Position, 
                transform.rotation * spawn.Rotation);
            asteroid.transform.localScale = transform.lossyScale.Multiply(spawn.Scale);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        foreach (var spawn in GenerateSpawns())
        {
            spawn.Prefab.gameObject.DrawPrefabWire(transform,
                spawn.Position,
                spawn.Rotation,
                spawn.Scale);
        }
    }
}
