using System;
using Microsoft.Xna.Framework;

namespace TopDownShooter.Game.Core;

public static class Extensions
{
    public static Vector3 Xz(this Vector3 v)
    {
        return new Vector3(v.X, 0, v.Z);
    }

    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.X, y, v.Z);
    }

    public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 normal)
    {
        return v - Vector3.Dot(v, normal) * normal;
    }

    public static bool RayPlaneY0(this Ray ray, out Vector3 hit)
    {
        hit = Vector3.Zero;

        if (Math.Abs(ray.Direction.Y) < 0.0001f)
            return false;

        var t = -ray.Position.Y / ray.Direction.Y;
        if (t < 0)
            return false;

        hit = ray.Position + t * ray.Direction;
        return true;
    }
}