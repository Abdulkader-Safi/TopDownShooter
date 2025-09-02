using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using BepuQuaternion = System.Numerics.Quaternion;

namespace Core.Physics;

public static class VectorConversions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToMonoGame(this System.Numerics.Vector3 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToBepuPhysics(this Vector3 vector)
    {
        return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToMonoGame(this System.Numerics.Vector2 vector)
    {
        return new Vector3(vector.X, 0f, vector.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToBepuPhysics2D(this Vector3 vector)
    {
        return new System.Numerics.Vector2(vector.X, vector.Z);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion ToMonoGame(this BepuQuaternion quaternion)
    {
        return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BepuQuaternion ToBepuPhysics(this Quaternion quaternion)
    {
        return new BepuQuaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }
}