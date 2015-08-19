using UnityEngine;
using UnityEngine.UI;

public class Predictor : MonoBehaviour
{
    [SerializeField]
    private PredictorMarker markerPrefab;

    //need a ref for a bracketmanager because we steal styles from it
    [SerializeField]
    private BracketManager brackets;

    /* doesn't need to be serialized as it's rebuilt if necessary */
    private PredictorMarker[] markers;

    private void ClearReticules()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        markers = null;
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
        var loadout = player.GetComponent<ModuleLoadout>();

        if (!loadout || !playerShip || !playerShip.Target)
        {
            ClearReticules();
            return;
        }

        var modCount = loadout.FrontModules.Size;
        var markerCount = 0;

        var newMarkerPositions = new Vector3[modCount];

        var playerTargetable = player.GetComponent<Targetable>();
        var playerFaction = playerTargetable ? playerTargetable.Faction : null;

        

        /* one marker per active hardpoint module which returns true for
        IsPredictable. we get the prediction points from the module's 
        behaviour here too */
        for (int moduleIndex = 0; moduleIndex < modCount; ++moduleIndex)
        {
            var module = loadout.FrontModules[moduleIndex];

            var behavior = module.Definition.Behaviour;
            var hardpoint = loadout.FindHardpoint(moduleIndex);

            var predictPoint = behavior.PredictTarget(playerShip, hardpoint, playerShip.Target);

            if (predictPoint.HasValue)
            {
                //PredictTarget returns world pos
                predictPoint = camera.WorldToScreenPoint(predictPoint.Value);

                int markerIndex = markerCount++;
                newMarkerPositions[markerIndex] = predictPoint.Value;
            }
        }

        /* if the number of markers has changed, we need to drop all our
        children and rebuild the marker list */
        if (markers == null || markers.Length != markerCount)
        {
            ClearReticules();
            markers = new PredictorMarker[markerCount];

            for (int marker = 0; marker < markerCount; ++marker)
            {
                var newMarker = Instantiate(markerPrefab);
                newMarker.transform.SetParent(transform, false);

                markers[marker] = newMarker;                
            }
        }

        var predictorColor = brackets.GetBracketColor(playerFaction, playerShip.Target);

        /* set positions of the (now correctly sized) marker list */
        for (int marker = 0; marker < markerCount; ++marker)
        {
            markers[marker].transform.position = newMarkerPositions[marker];
            markers[marker].Color = predictorColor;
        }
    }
}