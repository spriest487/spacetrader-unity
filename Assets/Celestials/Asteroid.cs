using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    public const string ASTEROID_TAG = "Asteroid";

    [Serializable]
    public class Deposit
    {
        public Hitpoints hitpoints;
        public Targetable targetable;
    }

    [SerializeField]
    private List<Targetable> deposits;
    
    private void Awake()
    {
        gameObject.tag = ASTEROID_TAG;

        var collider = GetComponentInChildren<Collider>();
        Debug.Assert(collider, "asteroid must have a collider child");

        //spin randomly
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.angularVelocity = new Vector3(
                Mathf.Deg2Rad * Random.Range(0, 15f),
                Mathf.Deg2Rad * Random.Range(0, 15f),
                Mathf.Deg2Rad * Random.Range(0, 15f));
        }

        deposits = new List<Targetable>();
                
        var count = Random.Range(1, 10);

        for (int i = 0; i < count; ++i)
        {
            var offset = Random.onUnitSphere * collider.bounds.extents.sqrMagnitude;
            var outerPoint = offset + transform.position;
            var surfacePos = transform.position;

            //fire a ray inwards to find our own surface
            foreach (var hit in Physics.RaycastAll(outerPoint, -offset, collider.bounds.extents.sqrMagnitude))
            {
                if (hit.collider != collider)
                {
                    continue;
                }

                surfacePos = hit.point;
                break;
            }
            
            var deposit = new GameObject("Deposit");
            var depositTargetable = deposit.AddComponent<Targetable>();
            depositTargetable.Faction = "resource";

            deposit.transform.position = surfacePos;
            deposit.transform.SetParent(transform, true);

            deposits.Add(depositTargetable);
        }
    }
}
