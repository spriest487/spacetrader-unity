#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerHotspot : MonoBehaviour
{
    private new Collider collider;

    private List<Ship> touchingShips;

    public Ship TouchingShip
    {
        get { return touchingShips.Count > 0 ? touchingShips[0] : null; }
    }

    public float Size { get; private set; }

    public event Action OnTouchingShipChanged;

    private void Awake()
    {
        collider = GetComponent<Collider>();

        var sphere = collider as SphereCollider;
        Size = sphere? sphere.radius * 2 : 1;
    }

    private void OnEnable()
    {
        touchingShips = new List<Ship>();
    }

    private void OnDisable()
    {
        touchingShips = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var ship = other.GetComponentInParent<Ship>();
        if (ship && !touchingShips.Contains(ship))
        {
            touchingShips.Add(ship);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ship = other.GetComponentInParent<Ship>();

        if (ship)
        {
            touchingShips.Remove(ship);
        }
    }
}
