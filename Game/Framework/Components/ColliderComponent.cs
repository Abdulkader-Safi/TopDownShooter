using Microsoft.Xna.Framework;
using TopDownShooter.Game.Core;

namespace TopDownShooter.Game.Framework.Components;

public enum ColliderType
{
    Box,
    Sphere,
    Capsule
}

public class ColliderComponent : IComponent
{
    public Entity Entity { get; set; }
    
    public ColliderType ColliderType { get; set; } = ColliderType.Box;
    public Vector3 Size { get; set; } = Vector3.One;
    public float Radius { get; set; } = 0.5f;
    public float Height { get; set; } = 1.0f;
    public Vector3 Offset { get; set; } = Vector3.Zero;
    
    private object _collidableDescription;
    private bool _isInitialized = false;
    
    public object CollidableDescription => _collidableDescription;
    public bool IsInitialized => _isInitialized;
    
    public void Initialize()
    {
        if (_isInitialized) return;
        
        var physics = GameRoot.Physics.PhysicsWorld;
        
        _collidableDescription = ColliderType switch
        {
            ColliderType.Box => physics.CreateBoxCollider(Size),
            ColliderType.Sphere => physics.CreateSphereCollider(Radius),
            ColliderType.Capsule => physics.CreateCapsuleCollider(Radius, Height),
            _ => throw new System.ArgumentOutOfRangeException()
        };
        
        _isInitialized = true;
    }
    
    public void SetAsBox(Vector3 size)
    {
        ColliderType = ColliderType.Box;
        Size = size;
        if (_isInitialized)
        {
            _collidableDescription = GameRoot.Physics.PhysicsWorld.CreateBoxCollider(size);
        }
    }
    
    public void SetAsSphere(float radius)
    {
        ColliderType = ColliderType.Sphere;
        Radius = radius;
        if (_isInitialized)
        {
            _collidableDescription = GameRoot.Physics.PhysicsWorld.CreateSphereCollider(radius);
        }
    }
    
    public void SetAsCapsule(float radius, float height)
    {
        ColliderType = ColliderType.Capsule;
        Radius = radius;
        Height = height;
        if (_isInitialized)
        {
            _collidableDescription = GameRoot.Physics.PhysicsWorld.CreateCapsuleCollider(radius, height);
        }
    }
}