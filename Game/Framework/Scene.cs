using System.Collections.Generic;
using System.Linq;

namespace TopDownShooter.Game.Framework;

public abstract class Scene
{
    private List<Entity> entities = [];
    
    public virtual void Initialize() { }
    public virtual void LoadContent() { }
    
    public virtual void Update()
    {
        for (var i = entities.Count - 1; i >= 0; i--)
        {
            if (i < entities.Count)
                entities[i].Update();
        }
    }
    
    public virtual void Draw()
    {
        foreach (var entity in entities)
            entity.Draw();
    }
    
    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
        entity.Scene = this;
    }
    
    public void RemoveEntity(Entity entity)
    {
        entities.Remove(entity);
        entity.Scene = null;
    }
    
    public T FindEntity<T>() where T : Entity
    {
        return entities.OfType<T>().FirstOrDefault();
    }
    
    public IEnumerable<T> FindEntities<T>() where T : Entity
    {
        return entities.OfType<T>();
    }
}