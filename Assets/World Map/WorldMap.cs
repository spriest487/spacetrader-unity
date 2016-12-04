#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class WorldMap : MonoBehaviour
{
    [SerializeField]
    private Camera mapCamera;

    [SerializeField]
    private Transform areasRoot;

    [SerializeField]
    private WorldMapMarker markerPrefab;

    //for want of a better place to put this
    [SerializeField]
    private AnimationCurve jumpEffectCurve;

    private List<WorldMapArea> areas;
    private List<WorldMapMarker> markers;

    public event Action<bool> OnVisibilityChanged;

    public AnimationCurve JumpEffectCurve
    {
        get { return jumpEffectCurve; }
    }

    public Camera Camera
    {
        get { return mapCamera; }
    }

    public bool Visible
    {
        get { return mapCamera.enabled; }
        set
        {
            mapCamera.enabled = value;

            var currentSkybox = BackgroundCamera.Current.GetComponent<Skybox>();
            if (currentSkybox)
            {
                mapCamera.clearFlags = CameraClearFlags.Skybox;
                mapCamera.GetComponent<Skybox>().material = currentSkybox.material;
            }
            else
            {
                mapCamera.clearFlags = CameraClearFlags.SolidColor;
            }
            OnVisibilityChanged(value);
        }
    }

    public IEnumerable<WorldMapArea> DistantAreas
    {
        get
        {
            if (areas == null)
            {
                Awake();
            }

            return areas
                .Where(a => a.name != SceneManager.GetActiveScene().name);
        }
    }

    public WorldMapArea GetCurrentArea()
    {
        if (areas == null)
        {
            Awake();
        }

        return areas
            .Where(a => a.name == SceneManager.GetActiveScene().name)
            .DefaultIfEmpty(areas[0])
            .First();
    }

    public WorldMapArea GetArea(string areaName)
    {
        return areas.Where(a => a.name == areaName).FirstOrDefault();
    }

    public bool IsWorldSceneActive
    {
        get
        {
            var currentScene = SceneManager.GetActiveScene();
            return areas.Any(a => a.name == currentScene.name);
        }
    }

    public IEnumerable<WorldMapMarker> Markers
    {
        get { return markers; }
    }

    private void Awake()
    {
        areasRoot.gameObject.SetActive(true);

        areas = GetComponentsInChildren<WorldMapArea>().ToList();
        markers = GetComponentsInChildren<WorldMapMarker>().ToList();

        //add default markers for those areas without preset ones
        var areasWithoutMarkers = areas.Except(markers.Select(m => m.Area))
            .ToList();
        markers.AddRange(areasWithoutMarkers.Select(a =>
            WorldMapMarker.Create(markerPrefab, a)));

        Camera.enabled = false;

        SceneManager.activeSceneChanged += (s1, s2) => CenterOnCurrentArea();
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

    public Coroutine LoadArea(WorldMapArea area)
    {
        return StartCoroutine(LoadAreaRoutine(area));
    }

    private IEnumerator LoadAreaRoutine(WorldMapArea area)
    {
        Debug.Assert(!MissionManager.Instance.Mission, "can't switch world scenes while in a mission");

        if (IsWorldSceneActive)
        {
            var currentArea = GetCurrentArea();

            SceneManager.SetActiveScene(SpaceTraderConfig.GlobalScene);
            yield return null;

            yield return SceneManager.UnloadSceneAsync(currentArea.name);
        }

        if (area)
        {
            yield return SceneManager.LoadSceneAsync(area.name, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(area.name));
            yield return null;
        }
    }
}