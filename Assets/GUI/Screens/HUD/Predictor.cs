#pragma warning disable 0649

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
    
    private PooledList<PredictorMarker, Vector3> markers;
    
    void LateUpdate()
    {
        if (markers == null)
        {
            markers = new PooledList<PredictorMarker, Vector3>(transform, markerPrefab);
        }

        var player = PlayerShip.LocalPlayer;
        var camera = FollowCamera.Current.Camera;
        Ship playerShip;

        if (!player 
            || !(playerShip = player.Ship) 
            || !player.Ship.Target 
            || player.Ship.Target.TargetSpace != TargetSpace.Local
            || !camera)
        {
            markers.Clear();
            return;
        }
        
        var loadout = playerShip.ModuleLoadout;
        var modCount = loadout.SlotCount;

        var newMarkerPositions = new List<Vector3>(modCount);
        //var markerItems = new List<HardpointModule>();

        var playerTargetable = playerShip.Targetable;
        
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
                if (predictPoint.HasValue && predictPoint.Value.z >= 0)
                {
                    newMarkerPositions.Add(predictPoint.Value);
                }
            }
        }

        var targetOrigin = player.Ship.Target.transform.position;
        float maxDistSqr = 0;
        for (int pos = 0; pos < newMarkerPositions.Count; ++pos)
        {
            float distSqr = (targetOrigin - newMarkerPositions[pos]).sqrMagnitude;
            maxDistSqr = Mathf.Max(maxDistSqr, distSqr);
        }

        if (maxDistSqr < MIN_SPEED)
        {
            markers.Clear();
        }
        else
        {
            var predictorColor = brackets.GetBracketColor(playerTargetable, playerShip.Target);
            markers.Refresh(newMarkerPositions, (i, marker, position) =>
            {
                //PredictTarget returns world pos
                var screenPos = camera.WorldToScreenPoint(position);

                marker.transform.position = screenPos;
                marker.Color = predictorColor;
            });
        }
    }
}