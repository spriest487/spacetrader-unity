using UnityEngine;
using System;

public class MathUtils : MonoBehaviour
{
	public static float NextFloat(System.Random random)
	{
		return (float) random.NextDouble();
	}

	public static bool SameSign(float f1, float f2)
	{
		return f1 * f2 >= 0.0f;
	}

    public static void OrderByDistance(Transform[] transforms, Vector3 point)
    {
        var result = new Transform[transforms.Length];
        
        Array.Sort(transforms, (a, b) =>
        {
            var aDist = (a.position - point).sqrMagnitude;
            var bDist = (b.position - point).sqrMagnitude;

            return (int) (aDist - bDist);
        });
    }
}
