using Microsoft.Xna.Framework;

namespace TopDownShooter.Game.Physics;

public class CapsuleCollider : ICollider
{
    public Vector3 Position { get; set; }
    public float Radius { get; set; } = 0.5f;
    public float HalfHeight { get; set; } = 1f;
    
    public BoundingBox GetBounds()
    {
        var extent = new Vector3(Radius, HalfHeight + Radius, Radius);
        return new BoundingBox(Position - extent, Position + extent);
    }
    
    public bool Intersects(ICollider other)
    {
        return other switch
        {
            CapsuleCollider capsule => IntersectsCapsule(capsule),
            AabbCollider aabb => aabb.Intersects(this),
            _ => false
        };
    }
    
    private bool IntersectsCapsule(CapsuleCollider other)
    {
        var thisTop = Position + Vector3.UnitY * HalfHeight;
        var thisBottom = Position - Vector3.UnitY * HalfHeight;
        var otherTop = other.Position + Vector3.UnitY * other.HalfHeight;
        var otherBottom = other.Position - Vector3.UnitY * other.HalfHeight;
        
        var distance = DistanceBetweenLineSegments(thisBottom, thisTop, otherBottom, otherTop);
        return distance <= (Radius + other.Radius);
    }
    
    private float DistanceBetweenLineSegments(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        var da = a2 - a1;
        var db = b2 - b1;
        var dc = b1 - a1;
        
        var dotA = Vector3.Dot(da, da);
        var dotB = Vector3.Dot(db, db);
        var dotC = Vector3.Dot(da, db);
        var dotD = Vector3.Dot(da, dc);
        var dotE = Vector3.Dot(db, dc);
        
        var s = 0f;
        var t = 0f;
        
        var denom = dotA * dotB - dotC * dotC;
        
        if (denom != 0f)
        {
            s = MathHelper.Clamp((dotC * dotE - dotB * dotD) / denom, 0f, 1f);
        }
        
        t = (dotC * s + dotE) / dotB;
        
        if (t < 0f)
        {
            t = 0f;
            s = MathHelper.Clamp(-dotD / dotA, 0f, 1f);
        }
        else if (t > 1f)
        {
            t = 1f;
            s = MathHelper.Clamp((dotC - dotD) / dotA, 0f, 1f);
        }
        
        var pointA = a1 + s * da;
        var pointB = b1 + t * db;
        
        return Vector3.Distance(pointA, pointB);
    }
}