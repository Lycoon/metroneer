using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
    private const double Epsilon = 1e-10;

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static bool IsZero(float d)
    {
        return Mathf.Abs(d) < Epsilon;
    }

    public static bool DoesIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2, out Vector2 intersection)
    {
        intersection = new Vector2();

        var r = p2 - p;
        var s = q2 - q;
        var rxs = Cross(r, s);
        var qpxr = Cross(q - p, r);

        if (IsZero(rxs) && !IsZero(qpxr))
            return false;

        var t = Cross(q - p, s) / rxs;
        var u = Cross(q - p, r) / rxs;

        if (!IsZero(rxs) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        {
            intersection = p + t * r;
            return true;
        }

        return false;
    }
}
