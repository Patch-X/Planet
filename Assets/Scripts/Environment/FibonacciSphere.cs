using UnityEngine;
using System.Collections.Generic;
public class FibonacciSphere
{
    public static List<Vector3> Generate(int count)
    {
        List<Vector3> points = new List<Vector3>();
        float offset = 2f / count;
        float increment = Mathf.PI * (3f - Mathf.Sqrt(5f)); // Golden angle ≈ 137.5°

        for (int i = 0; i < count; i++)
        {
            float y = ((i * offset) - 1f) + (offset / 2f);
            float r = Mathf.Sqrt(1f - y * y);

            float phi = i * increment;

            float x = Mathf.Cos(phi) * r;
            float z = Mathf.Sin(phi) * r;

            points.Add(new Vector3(x, y, z));
        }

        return points;
    }
}