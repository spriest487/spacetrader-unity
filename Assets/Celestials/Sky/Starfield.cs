using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Starfield : MonoBehaviour {
	private Color[] STARCOLORS = {
		new Color(0.62f, 0.69f, 0.96f),
		new Color(0.69f, 0.76f, 0.98f),
		new Color(0.80f, 0.84f, 0.98f),
		new Color(1.00f, 1.00f, 1.00f),
		new Color(0.98f, 0.95f, 0.92f),
		new Color(0.95f, 0.80f, 0.64f),
		new Color(0.95f, 0.78f, 0.46f),
	};
	
	public int starCount = 20000;
	public int seed = 123;
	
	private static Vector3 RandomStarPos(System.Random random)
	{
		var theta = 2f * Mathf.PI * MathUtils.NextFloat(random);

		var range = MathUtils.NextFloat(random);

		var result = new Vector3();
		result.x = Mathf.Cos(theta) * range;
		result.z = Mathf.Sin(theta) * range;

		result.y = (MathUtils.NextFloat(random) - 0.5f) * 2;
		bool posY = result.y > 0;
		result.y = Mathf.Pow(result.y, 2);
		result.y = posY ? result.y : -result.y;

		return result;
	}
	
	private Mesh Generate()
	{
		if (starCount >= 65536) {
			throw new UnityException("Number of stars must less than 65536");
		}

		var random = new System.Random(seed);
		
		Mesh mesh = new Mesh();
		var vertices = new Vector3[starCount];
		var indices = new int[starCount];
		var colors = new Color[starCount];

		for (var i = 0; i < starCount; ++i)
		{
			vertices[i] = RandomStarPos(random);

			indices[i] = i;

			colors[i] = STARCOLORS[random.Next(STARCOLORS.Length)];
			colors[i].a = (0.2f + 0.8f*MathUtils.NextFloat(random)) * (1 - (Mathf.Abs(vertices[i].y)));
		}

		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.SetIndices(indices, MeshTopology.Points, 0);

		return mesh;
	}
	
	public void Start()
	{
		GetComponent<MeshFilter>().mesh = Generate();
	}
}
