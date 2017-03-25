#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerHotspot : MonoBehaviour
{
    private new Collider collider;
    
    public Ship TouchingShip { get; private set; }

    public float Size { get; private set; }

    public event Action OnTouchingShipChanged;

    private void Awake()
    {
        collider = GetComponent<Collider>();

        var sphere = collider as SphereCollider;
        Size = sphere? sphere.radius * 2 : 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        var ship = other.GetComponentInParent<Ship>();
        if (ship && !TouchingShip)
        {
            TouchingShip = ship;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TouchingShip = null;
    }
}
