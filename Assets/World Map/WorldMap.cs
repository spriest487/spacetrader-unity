#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldMap : MonoBehaviour
{
    [SerializeField]
    private Camera mapCamera;

    [SerializeField]
    private Transform areasRoot;

    //for want of a better place to put this
    [SerializeField]
    private AnimationCurve jumpEffectCurve;

    private List<WorldMapArea> areas;
    
    public AnimationCurve JumpEffectCurve
    {
        get { return jumpEffectCurve; }
    }

    public Camera Camera
    {
        get { return mapCamera; }
    }

    public WorldMapArea GetCurrentArea()
    {
        if (areas == null)
        {
            Start();
        }

        return areas
            .Where(a => a.name == SceneManager.GetActiveScene().name)
            .FirstOrDefault();
    }

    private void Start()
    {
        areasRoot.gameObject.SetActive(true);
        areas = GetComponentsInChildren<WorldMapArea>().ToList();

        Camera.enabled = false;

        CenterOnCurrentArea();
    }

    private void OnLevelWasLoaded()
    {
        CenterOnCurrentArea();
    }

    private void CenterOnCurrentArea()
    {
        var currentArea = GetCurrentArea();

        foreach (var area in areas)
        {
            area.GetComponent<Targetable>().HideBracket = area == currentArea;
        }

        if (currentArea)
        {
            areasRoot.transform.position = -currentArea.transform.localPosition;
        }
        else
        {
            areasRoot.transform.position = Vector3.zero;
        }
    }
}