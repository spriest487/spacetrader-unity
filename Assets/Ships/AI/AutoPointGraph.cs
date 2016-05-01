using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class AutoPointGraph : MonoBehaviour
{
    private PointGraph graph;

    [SerializeField]
    private Transform points;

    void CreateGraph(AstarPath astar)
    {
        if (!points)
        {
            Debug.LogWarning("missing points root for autopointgraph " + name);
        }

        var graphs = new List<NavGraph>(astar.graphs);
        if (graphs.Contains(graph))
        {
            return;
        }

        graph = new PointGraph();
        graph.active = astar;
        graph.name = "AutoPointGraph (" + name + ")";
        graph.root = points;
        graph.raycast = true;
        graph.maxDistance = 0;
        graph.recursive = true;
        graph.mask = -1;
        
        graphs.Add(graph);
        astar.graphs = graphs.ToArray();
    }

    void Awake()
    {
        if (!points.gameObject.isStatic)
        {
            Debug.LogWarning("AutoPointGraph used with non-static root");
        }

        AstarPath.OnPreScan += CreateGraph;        
    }

    void OnDestroy()
    {
        AstarPath.OnPreScan -= CreateGraph;
    }
}