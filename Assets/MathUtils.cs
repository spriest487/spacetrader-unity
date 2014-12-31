using UnityEngine;
using System.Collections;

public class MathUtils : MonoBehaviour {
	public static float NextFloat(System.Random random)
	{
		return (float) random.NextDouble();
	}

	public static bool SameSign(float f1, float f2)
	{
		return f1 * f2 >= 0.0f;
	}
}
