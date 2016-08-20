using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Asteroid : MonoBehaviour
{
    [System.Serializable]
    public class Deposit
    {
        public Hitpoints hitpoints;
        public Targetable targetable;
    }

    //[SerializeField]
    //private int seed = new System.Random().Next(0, int.MaxValue);

    [SerializeField]
    private List<Targetable> deposits;

    //private Collider collider;

    private void Start()
    {
        var collider = GetComponent<Collider>();

        deposits = new List<Targetable>();

        var random = new System.Random();
        
        var count = random.Next(1, 10);

        for (int i = 0; i < count; ++i)
        {
            var offset = random.OnUnitSphere() * collider.bounds.extents.sqrMagnitude;
            var outerPoint = offset + transform.position;
            Vector3 surfacePos = transform.position;

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
