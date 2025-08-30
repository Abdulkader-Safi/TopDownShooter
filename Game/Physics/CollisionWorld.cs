using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TopDownShooter.Game.Physics.Spatial;

namespace TopDownShooter.Game.Physics;

public class CollisionWorld
{
    public static CollisionWorld Instance { get; private set; }
    
    private List<ICollider> _staticColliders = new List<ICollider>();
    private List<ICollider> _dynamicColliders = new List<ICollider>();
    private SpatialHash _spatialHash = new SpatialHash(1.0f);
    
    public CollisionWorld()
    {
        Instance = this;
    }
    
    public void AddStatic(ICollider collider)
    {
        _staticColliders.Add(collider);
        _spatialHash.Insert(collider);
    }
    
    public void AddDynamic(ICollider collider)
    {
        _dynamicColliders.Add(collider);
    }
    
    public void ClearDynamics()
    {
        _dynamicColliders.Clear();
    }
    
    public IEnumerable<ICollider> QueryAround(Vector3 position, float radius = 2f)
    {
        var candidates = _spatialHash.QueryRadius(position, radius);
        return candidates.Concat(_dynamicColliders);
    }
    
    public bool CastCapsule(Vector3 start, Vector3 end, float radius, float halfHeight, out Hit hit)
    {
        hit = new Hit();
        var direction = end - start;
        var distance = direction.Length();
        
        if (distance < 0.0001f)
            return false;
            
        direction /= distance;
        
        var testCapsule = new CapsuleCollider
        {
            Position = start,
            Radius = radius,
            HalfHeight = halfHeight
        };
        
        var stepSize = Math.Min(radius * 0.1f, 0.05f); // Much smaller steps for precision
        var steps = Math.Max(1, (int)(distance / stepSize) + 1);
        
        for (int i = 0; i <= steps; i++)
        {
            var t = MathHelper.Clamp((float)i / steps, 0f, 1f);
            testCapsule.Position = Vector3.Lerp(start, end, t);
            
            var nearby = QueryAround(testCapsule.Position, radius + 1f);
            
            foreach (var collider in nearby)
            {
                if (collider != testCapsule && testCapsule.Intersects(collider))
                {
                    // Calculate proper hit position - move back to contact point
                    var previousT = i > 0 ? (float)(i - 1) / steps : 0f;
                    hit.Position = Vector3.Lerp(start, end, previousT);
                    hit.Distance = previousT * distance;
                    hit.Collider = collider;
                    
                    // Calculate proper surface normal (from wall to capsule)
                    var toCapsule = testCapsule.Position - collider.Position;
                    toCapsule.Y = 0; // Keep movement on XZ plane for character movement
                    
                    if (toCapsule.LengthSquared() > 0.0001f)
                        hit.Normal = Vector3.Normalize(toCapsule);
                    else
                        hit.Normal = -direction; // Fallback: opposite of movement direction
                    
                    return true;
                }
            }
        }
        
        return false;
    }
    
    public bool OverlapAabb(BoundingBox a, BoundingBox b)
    {
        return a.Intersects(b);
    }
    
    public IEnumerable<ICollider> GetAllStatic()
    {
        return _staticColliders;
    }
    
    public IEnumerable<ICollider> GetAllDynamic()
    {
        return _dynamicColliders;
    }
}