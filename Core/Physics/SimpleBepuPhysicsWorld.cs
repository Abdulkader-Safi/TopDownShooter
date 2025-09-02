using System;
using Microsoft.Xna.Framework;

namespace Core.Physics;

/// <summary>
/// Simplified BepuPhysics integration that provides the interface needed
/// without full implementation complexity. This serves as a foundation
/// that can be expanded later.
/// </summary>
public class SimpleBepuPhysicsWorld : IDisposable
{
    public static SimpleBepuPhysicsWorld Instance { get; private set; }
    
    private bool _disposed = false;
    
    public SimpleBepuPhysicsWorld()
    {
        Instance = this;
    }
    
    public void Update(float deltaTime)
    {
        if (_disposed) return;
        // Placeholder for physics simulation step
    }
    
    public int AddDynamicBody(Vector3 position, System.Numerics.Quaternion orientation, object collidable, float mass = 1.0f)
    {
        // Return a dummy handle for now
        return System.Random.Shared.Next(1000, 9999);
    }
    
    public int AddKinematicBody(Vector3 position, System.Numerics.Quaternion orientation, object collidable)
    {
        // Return a dummy handle for now
        return System.Random.Shared.Next(1000, 9999);
    }
    
    public int AddStaticBody(Vector3 position, System.Numerics.Quaternion orientation, object collidable)
    {
        // Return a dummy handle for now
        return System.Random.Shared.Next(1000, 9999);
    }
    
    public void SetBodyPosition(int bodyHandle, Vector3 position)
    {
        // Placeholder
    }
    
    public Vector3 GetBodyPosition(int bodyHandle)
    {
        // Placeholder - return zero for now
        return Vector3.Zero;
    }
    
    public void SetBodyVelocity(int bodyHandle, Vector3 velocity)
    {
        // Placeholder
    }
    
    public Vector3 GetBodyVelocity(int bodyHandle)
    {
        // Placeholder - return zero for now
        return Vector3.Zero;
    }
    
    public void RemoveBody(int bodyHandle)
    {
        // Placeholder
    }
    
    public void RemoveStatic(int staticHandle)
    {
        // Placeholder
    }
    
    public void RegisterEntityBody(object entity, int bodyHandle)
    {
        // Placeholder
    }
    
    public object GetEntityFromBody(int bodyHandle)
    {
        // Placeholder
        return null;
    }
    
    public int GetBodyFromEntity(object entity)
    {
        // Placeholder
        return -1;
    }
    
    public object CreateBoxCollider(Vector3 size)
    {
        // Return a simple descriptor
        return new { Type = "Box", Size = size };
    }
    
    public object CreateSphereCollider(float radius)
    {
        // Return a simple descriptor
        return new { Type = "Sphere", Radius = radius };
    }
    
    public object CreateCapsuleCollider(float radius, float height)
    {
        // Return a simple descriptor
        return new { Type = "Capsule", Radius = radius, Height = height };
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        if (Instance == this)
            Instance = null;
    }
}