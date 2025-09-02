using Microsoft.Xna.Framework;

namespace Core.Physics;

public interface ICollider
{
    Vector3 Position { get; set; }
    BoundingBox GetBounds();
    bool Intersects(ICollider other);
}

public struct Hit
{
    public Vector3 Position;
    public Vector3 Normal;
    public float Distance;
    public ICollider Collider;
}