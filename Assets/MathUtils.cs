using UnityEngine;
using System;

public static class MathUtils
{
	public static float NextFloat(this System.Random random)
	{
		return (float) random.NextDouble();
	}

	public static bool SameSign(float f1, float f2)
	{
		return f1 * f2 >= 0.0f;
	}

    public static void OrderByDistance(Transform[] transforms, Vector3 point)
    {
        Array.Sort(transforms, (a, b) =>
        {
            var aDist = (a.position - point).sqrMagnitude;
            var bDist = (b.position - point).sqrMagnitude;

            return (int) (aDist - bDist);
        });
    }

    public static Vector3 OnUnitSphere(this System.Random random)
    {
        var phi = random.NextDouble() * Math.PI * 2;
        var cosTheta = random.NextDouble() * 2 - 1;

        var theta = Math.Acos(cosTheta);

        var x = Math.Sin(theta) * Math.Cos(phi);
        var y = Math.Sin(theta) * Math.Sin(phi);
        var z = Math.Cos(theta);

        return new Vector3((float) x, (float) y, (float) z);
    }

    public static float MaxAbs(params float[] vals)
    {
        float max = 0;
        foreach (var x in vals)
        {
            if (Mathf.Abs(x) > Mathf.Abs(max))
            {
                max = x;
            }
        }
        return max;
    }

    public static Vector3 Multiply(this Vector3 left, Vector3 right)
    {
        return new Vector3(left.x * right.x,
            left.y * right.y,
            left.z * right.z);
    }
}
