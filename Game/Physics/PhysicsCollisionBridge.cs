using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Core;

namespace TopDownShooter.Game.Physics;

/// <summary>
/// Bridge between BepuPhysics and the existing collision system.
/// Allows gradual transition and maintains compatibility with existing collision code.
/// </summary>
public class PhysicsCollisionBridge
{
    private readonly Dictionary<Entity, BepuPhysicsCollider> _entityColliders = new();
    
    /// <summary>
    /// Register an entity with physics components as a physics-based collider
    /// </summary>
    public void RegisterPhysicsEntity(Entity entity)
    {
        if (!entity.HasPhysicsBody() || !entity.HasCollider()) return;
        
        var collider = new BepuPhysicsCollider(entity);
        _entityColliders[entity] = collider;
        
        var rigidBody = entity.GetRigidBody();
        if (rigidBody.BodyType == BodyType.Static)
        {
            GameRoot.Physics.CollisionWorld.AddStatic(collider);
        }
        else
        {
            GameRoot.Physics.CollisionWorld.AddDynamic(collider);
        }
    }
    
    /// <summary>
    /// Unregister a physics entity from collision system
    /// </summary>
    public void UnregisterPhysicsEntity(Entity entity)
    {
        if (_entityColliders.TryGetValue(entity, out var collider))
        {
            _entityColliders.Remove(entity);
            // Note: CollisionWorld doesn't have Remove methods, so we'll need to add them or handle differently
        }
    }
    
    /// <summary>
    /// Update all physics-based colliders positions from their BepuPhysics bodies
    /// </summary>
    public void UpdatePhysicsColliders()
    {
        foreach (var kvp in _entityColliders)
        {
            var entity = kvp.Key;
            var collider = kvp.Value;
            var rigidBody = entity.GetRigidBody();
            
            if (rigidBody?.IsInitialized == true)
            {
                collider.Position = rigidBody.Position;
            }
        }
    }
    
    /// <summary>
    /// Get collision information for a physics entity at a specific position
    /// </summary>
    public bool TestPhysicsEntityCollision(Entity entity, Vector3 testPosition, out Hit hit)
    {
        hit = new Hit();
        
        if (!_entityColliders.TryGetValue(entity, out var collider))
            return false;
        
        var originalPosition = collider.Position;
        collider.Position = testPosition;
        
        var nearby = GetNearbyColliders(testPosition, 2.0f);
        foreach (var other in nearby)
        {
            if (other == collider) continue;
            
            if (collider.Intersects(other))
            {
                hit.Position = testPosition;
                hit.Collider = other;
                hit.Distance = Vector3.Distance(originalPosition, testPosition);
                
                // Calculate normal from other collider to this one
                var toEntity = testPosition - other.Position;
                if (toEntity.LengthSquared() > 0.0001f)
                {
                    hit.Normal = Vector3.Normalize(toEntity);
                }
                
                collider.Position = originalPosition;
                return true;
            }
        }
        
        collider.Position = originalPosition;
        return false;
    }
    
    private IEnumerable<ICollider> GetNearbyColliders(Vector3 position, float radius)
    {
        // Use the existing spatial hash system to get nearby colliders
        var statics = GameRoot.Physics.CollisionWorld.GetAllStatic();
        var dynamics = GameRoot.Physics.CollisionWorld.GetAllDynamic();
        
        foreach (var collider in statics)
        {
            if (Vector3.Distance(collider.Position, position) <= radius)
                yield return collider;
        }
        
        foreach (var collider in dynamics)
        {
            if (Vector3.Distance(collider.Position, position) <= radius)
                yield return collider;
        }
    }
}

/// <summary>
/// Adapter class that makes BepuPhysics entities compatible with the existing ICollider system
/// </summary>
public class BepuPhysicsCollider : ICollider
{
    private readonly Entity _entity;
    
    public Vector3 Position { get; set; }
    
    public BepuPhysicsCollider(Entity entity)
    {
        _entity = entity;
        Position = entity.Transform.Position;
    }
    
    public BoundingBox GetBounds()
    {
        var colliderComponent = _entity.GetCollider();
        if (colliderComponent == null) 
            return new BoundingBox(Position - Vector3.One * 0.5f, Position + Vector3.One * 0.5f);
        
        return colliderComponent.ColliderType switch
        {
            ColliderType.Box => GetBoxBounds(colliderComponent.Size),
            ColliderType.Sphere => GetSphereBounds(colliderComponent.Radius),
            ColliderType.Capsule => GetCapsuleBounds(colliderComponent.Radius, colliderComponent.Height),
            _ => new BoundingBox(Position - Vector3.One * 0.5f, Position + Vector3.One * 0.5f)
        };
    }
    
    private BoundingBox GetBoxBounds(Vector3 size)
    {
        var halfSize = size * 0.5f;
        return new BoundingBox(Position - halfSize, Position + halfSize);
    }
    
    private BoundingBox GetSphereBounds(float radius)
    {
        var radiusVec = Vector3.One * radius;
        return new BoundingBox(Position - radiusVec, Position + radiusVec);
    }
    
    private BoundingBox GetCapsuleBounds(float radius, float height)
    {
        var halfHeight = height * 0.5f + radius;
        var bounds = new Vector3(radius, halfHeight, radius);
        return new BoundingBox(Position - bounds, Position + bounds);
    }
    
    public bool Intersects(ICollider other)
    {
        // For now, use simple bounding box intersection
        // This could be enhanced with more precise collision detection later
        return GetBounds().Intersects(other.GetBounds());
    }
}