using Microsoft.Xna.Framework;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;

namespace TopDownShooter.Game.Physics;

public static class PhysicsBodyFactory
{
    /// <summary>
    /// Creates a dynamic physics body with a box collider
    /// </summary>
    public static void CreateDynamicBox(Entity entity, Vector3 size, float mass = 1.0f)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsBox(size);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Dynamic;
        rigidBody.Mass = mass;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a dynamic physics body with a sphere collider
    /// </summary>
    public static void CreateDynamicSphere(Entity entity, float radius, float mass = 1.0f)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsSphere(radius);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Dynamic;
        rigidBody.Mass = mass;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a dynamic physics body with a capsule collider
    /// </summary>
    public static void CreateDynamicCapsule(Entity entity, float radius, float height, float mass = 1.0f)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsCapsule(radius, height);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Dynamic;
        rigidBody.Mass = mass;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a kinematic physics body with a box collider (moves but not affected by physics)
    /// </summary>
    public static void CreateKinematicBox(Entity entity, Vector3 size)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsBox(size);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Kinematic;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a kinematic physics body with a sphere collider (moves but not affected by physics)
    /// </summary>
    public static void CreateKinematicSphere(Entity entity, float radius)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsSphere(radius);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Kinematic;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a kinematic physics body with a capsule collider (moves but not affected by physics)
    /// </summary>
    public static void CreateKinematicCapsule(Entity entity, float radius, float height)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsCapsule(radius, height);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Kinematic;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a static physics body with a box collider (immovable)
    /// </summary>
    public static void CreateStaticBox(Entity entity, Vector3 size)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsBox(size);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Static;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a static physics body with a sphere collider (immovable)
    /// </summary>
    public static void CreateStaticSphere(Entity entity, float radius)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsSphere(radius);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Static;
        rigidBody.Initialize(collider.CollidableDescription);
    }
    
    /// <summary>
    /// Creates a static physics body with a capsule collider (immovable)
    /// </summary>
    public static void CreateStaticCapsule(Entity entity, float radius, float height)
    {
        var collider = entity.AddComponent<ColliderComponent>();
        collider.SetAsCapsule(radius, height);
        collider.Initialize();
        
        var rigidBody = entity.AddComponent<RigidBodyComponent>();
        rigidBody.BodyType = BodyType.Static;
        rigidBody.Initialize(collider.CollidableDescription);
    }
}