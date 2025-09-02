using System;
using System.Collections.Generic;
using Core.Framework.Components;
using Core.Physics;

namespace Core.Framework;

public class Entity
{
    private readonly Dictionary<Type, object> _components = new Dictionary<Type, object>();

    public Scene Scene { get; set; }
    public Transform Transform { get; private set; }

    public Entity()
    {
        Transform = AddComponent<Transform>();
    }

    public T AddComponent<T>() where T : class, new()
    {
        var component = new T();
        _components[typeof(T)] = component;

        if (component is IComponent iComponent)
            iComponent.Entity = this;

        return component;
    }

    public T GetComponent<T>() where T : class
    {
        _components.TryGetValue(typeof(T), out var component);
        return component as T;
    }

    public bool HasComponent<T>() where T : class
    {
        return _components.ContainsKey(typeof(T));
    }

    public virtual void Update()
    {
        foreach (var component in _components.Values)
        {
            if (component is IUpdateable updateable)
                updateable.Update();
        }
    }

    public virtual void Draw()
    {
        foreach (var component in _components.Values)
        {
            if (component is IDrawable drawable)
                drawable.Draw();
        }
    }

    // Physics helper methods
    public RigidBodyComponent GetRigidBody() => GetComponent<RigidBodyComponent>();
    public ColliderComponent GetCollider() => GetComponent<ColliderComponent>();

    public bool HasPhysicsBody() => HasComponent<RigidBodyComponent>();
    public bool HasCollider() => HasComponent<ColliderComponent>();

    // Physics body creation shortcuts
    public void CreateDynamicBox(Microsoft.Xna.Framework.Vector3 size, float mass = 1.0f)
        => PhysicsBodyFactory.CreateDynamicBox(this, size, mass);

    public void CreateDynamicSphere(float radius, float mass = 1.0f)
        => PhysicsBodyFactory.CreateDynamicSphere(this, radius, mass);

    public void CreateDynamicCapsule(float radius, float height, float mass = 1.0f)
        => PhysicsBodyFactory.CreateDynamicCapsule(this, radius, height, mass);

    public void CreateKinematicBox(Microsoft.Xna.Framework.Vector3 size)
        => PhysicsBodyFactory.CreateKinematicBox(this, size);

    public void CreateKinematicSphere(float radius)
        => PhysicsBodyFactory.CreateKinematicSphere(this, radius);

    public void CreateKinematicCapsule(float radius, float height)
        => PhysicsBodyFactory.CreateKinematicCapsule(this, radius, height);

    public void CreateStaticBox(Microsoft.Xna.Framework.Vector3 size)
        => PhysicsBodyFactory.CreateStaticBox(this, size);

    public void CreateStaticSphere(float radius)
        => PhysicsBodyFactory.CreateStaticSphere(this, radius);

    public void CreateStaticCapsule(float radius, float height)
        => PhysicsBodyFactory.CreateStaticCapsule(this, radius, height);

    // Cleanup physics components when entity is destroyed
    public virtual void Destroy()
    {
        var rigidBody = GetRigidBody();
        rigidBody?.Destroy();
    }
}