using Microsoft.Xna.Framework;

namespace Core.Physics;

public class AabbCollider : ICollider
{
    public Vector3 Position { get; set; }
    public Vector3 Size { get; init; } = Vector3.One;
    
    public BoundingBox GetBounds()
    {
        var halfSize = Size * 0.5f;
        return new BoundingBox(Position - halfSize, Position + halfSize);
    }
    
    public bool Intersects(ICollider other)
    {
        return other switch
        {
            AabbCollider aabb => GetBounds().Intersects(aabb.GetBounds()),
            CapsuleCollider capsule => IntersectsCapsule(capsule),
            _ => false
        };
    }
    
    private bool IntersectsCapsule(CapsuleCollider capsule)
    {
        var bounds = GetBounds();
        var capsuleTop = capsule.Position + Vector3.UnitY * capsule.HalfHeight;
        var capsuleBottom = capsule.Position - Vector3.UnitY * capsule.HalfHeight;
        
        // Find closest point on box to the capsule's central axis
        var closestPointOnBox = GetClosestPointOnBox(bounds, capsule.Position);
        
        // Calculate distance from closest box point to capsule axis
        var distance = DistancePointToLineSegment(closestPointOnBox, capsuleBottom, capsuleTop);
        
        return distance <= capsule.Radius;
    }
    
    private Vector3 ClosestPointOnLineSegment(Vector3 start, Vector3 end, BoundingBox box)
    {
        // This is simplified - find closest point on box to the line segment
        var center = (box.Min + box.Max) * 0.5f;
        var lineCenter = (start + end) * 0.5f;
        
        var clampedX = MathHelper.Clamp(lineCenter.X, box.Min.X, box.Max.X);
        var clampedY = MathHelper.Clamp(lineCenter.Y, box.Min.Y, box.Max.Y);
        var clampedZ = MathHelper.Clamp(lineCenter.Z, box.Min.Z, box.Max.Z);
        
        return new Vector3(clampedX, clampedY, clampedZ);
    }
    
    private Vector3 GetClosestPointOnBox(BoundingBox box, Vector3 point)
    {
        var clampedX = MathHelper.Clamp(point.X, box.Min.X, box.Max.X);
        var clampedY = MathHelper.Clamp(point.Y, box.Min.Y, box.Max.Y);
        var clampedZ = MathHelper.Clamp(point.Z, box.Min.Z, box.Max.Z);
        
        return new Vector3(clampedX, clampedY, clampedZ);
    }
    
    private float DistancePointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        var lineDirection = lineEnd - lineStart;
        var lineLength = lineDirection.Length();
        
        if (lineLength < 0.0001f)
            return Vector3.Distance(point, lineStart);
            
        lineDirection /= lineLength;
        
        var t = Vector3.Dot(point - lineStart, lineDirection);
        t = MathHelper.Clamp(t, 0f, lineLength);
        
        var closestPoint = lineStart + lineDirection * t;
        return Vector3.Distance(point, closestPoint);
    }
}