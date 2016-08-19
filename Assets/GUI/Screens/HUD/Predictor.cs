﻿#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class Predictor : MonoBehaviour
{
    const float MIN_SPEED = 0.01f;

    [SerializeField]
    private PredictorMarker markerPrefab;

    //need a ref for a bracketmanager because we steal styles from it
    [SerializeField]
    private BracketManager brackets;

    /* doesn't need to be serialized as it's rebuilt if necessary */
    private List<PredictorMarker> markers;

    private void ClearReticules()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        markers = new List<PredictorMarker>();
    }

    void LateUpdate()
    {
        var player = PlayerShip.LocalPlayer;
        var camera = Camera.main;

        if (!player || !camera)
        {
            ClearReticules();
            return;
        }

        var playerShip = player.GetComponent<Ship>();

        if (!playerShip || !playerShip.Target)
        {
            ClearReticules();
            return;
        }

        var loadout = playerShip.ModuleLoadout;
        var modCount = loadout.SlotCount;

        var newMarkerPositions = new List<Vector3>(modCount);

        var playerTargetable = player.GetComponent<Targetable>();
        
        /* one marker per active hardpoint module which returns true for
        IsPredictable. we get the prediction points from the module's 
        behaviour here too */
        for (int moduleIndex = 0; moduleIndex < modCount; ++moduleIndex)
        {
            var module = loadout.GetSlot(moduleIndex);
            if (!module.ModuleType)
            {
                //empty slot
                continue;
            }

            var behavior = module.ModuleType.Behaviour;

            var predictPoint = behavior.PredictTarget(playerShip, moduleIndex, playerShip.Target);

            if (predictPoint.HasValue)
            {
                //PredictTarget returns world pos
                predictPoint = camera.WorldToScreenPoint(predictPoint.Value);

                if (predictPoint.HasValue && predictPoint.Value.z >= 0)
                {
                    newMarkerPositions.Add(predictPoint.Value);
                }
            }
        }

        /* if the number of markers has changed, we need to drop all our
        children and rebuild the marker list */
        var markerCount = newMarkerPositions.Count;
        if (markers == null || markers.Count != markerCount)
        {
            ClearReticules();

            for (int marker = 0; marker < newMarkerPositions.Count; ++marker)
            {
                var newMarker = Instantiate(markerPrefab);
                newMarker.transform.SetParent(transform, false);

                markers.Add(newMarker);
            }
        }

        var predictorColor = brackets.GetBracketColor(playerTargetable, playerShip.Target);

        /* set positions of the (now correctly sized) marker list */
        for (int marker = 0; marker < markerCount; ++marker)
        {
            markers[marker].transform.position = newMarkerPositions[marker];
            markers[marker].Color = predictorColor;
        }
    }
}