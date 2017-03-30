using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class Scanner : MonoBehaviour
{
    public struct Hit
    {
        public Targetable Targetable { get; private set; }
        public int Threat { get; private set; }

        public Hit(Targetable targetable, int threat)
        {
            Targetable = targetable;
            Threat = threat;
        }
    }

    const int maxHits = 10;
    const float range = 100;

    private Collider[] hitColliders;
    private int count;
    private Hit[] hits;

    public Ship Ship { get; private set; }

    public static float Range { get { return range; } }

    private void Awake()
    {
        Ship = GetComponentInParent<Ship>();
        count = -1;
    }

    private void OnEnable()
    {
        count = -1;
    }

    private void Update()
    {
        //forget results from last frame
        count = -1;
    }

    public int HitCount
    {
        get
        {
            UpdateHits();
            return count;
        }
    }

    public Hit GetHit(int hit)
    {
        UpdateHits();
        return hits[hit];
    }

    public Hit? FindHighestThreat()
    {
        UpdateHits();

        int maxThreat = 0;
        int highest = -1;

        for (int hit = 0; hit < hits.Length; ++hit)
        {
            if (hits[hit].Threat > maxThreat)
            {
                maxThreat = hits[hit].Threat;
                highest = hit;
            }
        }

        return highest == -1 ? (Hit?)null : hits[highest];
    }

    private void UpdateHits()
    {
        /* only do the query again once per frame/when the dirty flag is reset */
        if (count != -1)
        {
            return;
        }

        if (hitColliders == null || hits == null)
        {
            hitColliders = new Collider[maxHits];
            hits = new Hit[maxHits];
        }

        var layerMask = LayerMask.NameToLayer("Ships");

        count = Physics.OverlapSphereNonAlloc(transform.position, range, hitColliders, layerMask, QueryTriggerInteraction.Ignore);

        int hit = 0;
        for (int collision = 0; collision < count; ++collision)
        {
            //skip myself
            if (hitColliders[collision] == Ship.Collider)
            {
                continue;
            }

            //skip non-targetable ships
            var target = hitColliders[collision].GetComponent<Targetable>();
            if (!target)
            {
                continue;
            }

            var threat = CalculateThreat(target);

            hits[hit] = new Hit(target, threat);
            ++hit;
        }
    }

    private int CalculateThreat(Targetable target)
    {
        int threat = 1;

        //invert threat for friendlies
        if (Ship.Targetable
            && Ship.Targetable.RelationshipTo(target) != TargetRelationship.Hostile)
        {
            threat = -threat;
        }

        float comfortDistance = range * 0.5f;
        
        var dist2 = (transform.position - target.transform.position).sqrMagnitude;
        if (dist2 < (comfortDistance * comfortDistance))
        {
            threat *= 2;
        }

        return threat;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
