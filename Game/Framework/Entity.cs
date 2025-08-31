using System;
using System.Collections.Generic;
using TopDownShooter.Game.Framework.Components;

namespace TopDownShooter.Game.Framework;

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
}