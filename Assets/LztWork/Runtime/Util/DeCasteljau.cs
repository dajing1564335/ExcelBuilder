using System.Collections.Generic;
using UnityEngine;

public static class DeCasteljau
{
    public static float ComputePoint(List<float> controlPoint, float t)
    {
        var points = new List<float>(controlPoint);
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < points.Count - 1 - i; j++)
            {
                points[j] = (1 - t) * points[j] + t * points[j + 1];
            }
        }
        return points[0];
    }

    public static Vector2 ComputePoint(List<Vector2> controlPoint, float t)
    {
        var points = new List<Vector2>(controlPoint);
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < points.Count - 1 - i; j++)
            {
                points[j] = (1 - t) * points[j] + t * points[j + 1];
            }
        }
        return points[0];
    }

    public static Vector3 ComputePoint(List<Vector3> controlPoint, float t)
    {
        var points = new List<Vector3>(controlPoint);
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < points.Count - 1 - i; j++)
            {
                points[j] = (1 - t) * points[j] + t * points[j + 1];
            }
        }
        return points[0];
    }

    public static Color ComputePoint(List<Color> controlPoint, float t)
    {
        var points = new List<Color>(controlPoint);
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < points.Count - 1 - i; j++)
            {
                points[j] = (1 - t) * points[j] + t * points[j + 1];
            }
        }
        return points[0];
    }
}
