using UnityEngine;
using UnityEditor;

public class StarfieldMeshGenerator : EditorWindow
{
    private static readonly Color[] starColors = 
    {
        new Color(0.62f, 0.69f, 0.96f),
        new Color(0.69f, 0.76f, 0.98f),
        new Color(0.80f, 0.84f, 0.98f),
        new Color(1.00f, 1.00f, 1.00f),
        new Color(0.98f, 0.95f, 0.92f),
        new Color(0.95f, 0.80f, 0.64f),
        new Color(0.95f, 0.78f, 0.46f),
    };

    [SerializeField]
    private int starCount = 20000;

    [SerializeField]
    private int seed = 123;

    [SerializeField]
    private string path = "Assets/Celestials/Sky/New starfield.asset";

    private Mesh Generate()
    {
        Debug.Assert(starCount < 65536, "Number of stars must less than 65536");

        var random = new System.Random(seed);

        var mesh = new Mesh();
        var vertices = new Vector3[starCount];
        var indices = new int[starCount];
        var colors = new Color[starCount];

        for (var i = 0; i < starCount; ++i)
        {
            vertices[i] = random.OnUnitSphere();

            indices[i] = i;

            colors[i] = starColors[random.Next(starColors.Length)];
            colors[i].a = (0.2f + 0.8f * MathUtils.NextFloat(random)) * (1 - (Mathf.Abs(vertices[i].y)));
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);

        return mesh;
    }

    void OnEnable()
    {
        EditorPrefs.SetInt("SpaceTrader.StarfieldMeshGenerator.StarCount", starCount);
        EditorPrefs.SetInt("SpaceTrader.StarfieldMeshGenerator.Seed", seed);
        EditorPrefs.SetString("SpaceTrader.StarfieldMeshGenerator.Path", path);
    }

    void OnDisable()
    {
        starCount = EditorPrefs.GetInt("SpaceTrader.StarfieldMeshGenerator.StarCount");
        seed = EditorPrefs.GetInt("SpaceTrader.StarfieldMeshGenerator.Seed");
        path = EditorPrefs.GetString("SpaceTrader.StarfieldMeshGenerator.Path");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Asset");
        path = EditorGUILayout.TextField("Output Path", path);

        EditorGUILayout.LabelField("Generation Parameters");
        starCount = EditorGUILayout.IntField("Star Count", starCount);
        seed = EditorGUILayout.IntField("Random Seed", seed);

        if (GUILayout.Button("Generate"))
        {
            var mesh = Generate();
            
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("SpaceTrader/Starfield Mesh Generator")]
    public static void ShowWindow()
    {
        GetWindow<StarfieldMeshGenerator>(true, "Starfield Mesh Generator", true)
            .Show();
    }
}

