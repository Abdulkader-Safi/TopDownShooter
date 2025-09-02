using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.Framework;
using Core.Framework.Components;
using Core.Rendering;

namespace Core.Physics;

/// <summary>
/// Debug visualization for physics bodies and collision shapes
/// </summary>
public static class PhysicsDebugRenderer
{
    public static bool DrawPhysicsBodies { get; set; } = false;
    public static bool DrawVelocityVectors { get; set; } = false;
    public static bool DrawContactPoints { get; set; } = false;

    // Colors for different body types
    public static Color StaticBodyColor { get; set; } = Color.Blue;
    public static Color KinematicBodyColor { get; set; } = Color.Orange;
    public static Color DynamicBodyColor { get; set; } = Color.Green;
    public static Color SleepingBodyColor { get; set; } = Color.Gray;
    public static Color VelocityVectorColor { get; set; } = Color.Red;
    public static Color ContactPointColor { get; set; } = Color.Yellow;

    /// <summary>
    /// Draw debug visualization for all physics entities in the scene
    /// </summary>
    public static void DrawScene(IEnumerable<Entity> entities)
    {
        if (!DebugDraw.IsEnabled) return;

        foreach (var entity in entities)
        {
            if (entity.HasPhysicsBody())
            {
                DrawPhysicsEntity(entity);
            }
        }
    }

    /// <summary>
    /// Draw debug visualization for a single physics entity
    /// </summary>
    public static void DrawPhysicsEntity(Entity entity)
    {
        if (!DebugDraw.IsEnabled || !entity.HasPhysicsBody() || !entity.HasCollider())
            return;

        var rigidBody = entity.GetRigidBody();
        var collider = entity.GetCollider();

        if (!rigidBody.IsInitialized || !collider.IsInitialized)
            return;

        var position = rigidBody.Position;
        var color = GetBodyColor(rigidBody.BodyType, rigidBody.IsEnabled);

        // Draw physics body shape
        if (DrawPhysicsBodies)
        {
            DrawColliderShape(position, collider, color);
        }

        // Draw velocity vector
        if (DrawVelocityVectors && rigidBody.BodyType == BodyType.Dynamic)
        {
            var velocity = rigidBody.Velocity;
            if (velocity.LengthSquared() > 0.01f) // Only draw if moving
            {
                var velocityEnd = position + velocity * 0.5f; // Scale for visibility
                DebugDraw.DrawLine(position, velocityEnd, VelocityVectorColor);

                // Draw velocity magnitude as text would require SpriteBatch, so we'll use a simple arrow
                DrawVelocityArrow(position, velocity);
            }
        }
    }

    private static void DrawColliderShape(Vector3 position, ColliderComponent collider, Color color)
    {
        switch (collider.ColliderType)
        {
            case ColliderType.Box:
                DrawBoxCollider(position + collider.Offset, collider.Size, color);
                break;

            case ColliderType.Sphere:
                DrawSphereCollider(position + collider.Offset, collider.Radius, color);
                break;

            case ColliderType.Capsule:
                DrawCapsuleCollider(position + collider.Offset, collider.Radius, collider.Height, color);
                break;
        }
    }

    private static void DrawBoxCollider(Vector3 position, Vector3 size, Color color)
    {
        var halfSize = size * 0.5f;
        var bounds = new BoundingBox(position - halfSize, position + halfSize);
        DebugDraw.DrawBox(bounds, color);
    }

    private static void DrawSphereCollider(Vector3 position, float radius, Color color)
    {
        DebugDraw.DrawSphere(position, radius, color);
    }

    private static void DrawCapsuleCollider(Vector3 position, float radius, float height, Color color)
    {
        var halfHeight = height * 0.5f;
        DebugDraw.DrawCapsule(position, radius, halfHeight, color);
    }

    private static void DrawVelocityArrow(Vector3 position, Vector3 velocity)
    {
        if (velocity.LengthSquared() < 0.01f) return;

        var velocityNormalized = Vector3.Normalize(velocity);
        var velocityEnd = position + velocity * 0.5f;

        // Draw arrow head
        var arrowSize = 0.1f;
        var right = Vector3.Cross(velocityNormalized, Vector3.Up);
        var up = Vector3.Cross(right, velocityNormalized);

        if (right.LengthSquared() < 0.01f)
        {
            right = Vector3.Cross(velocityNormalized, Vector3.Forward);
            up = Vector3.Cross(right, velocityNormalized);
        }

        right = Vector3.Normalize(right) * arrowSize;
        up = Vector3.Normalize(up) * arrowSize;

        // Draw arrow head lines
        DebugDraw.DrawLine(velocityEnd, velocityEnd - velocityNormalized * arrowSize + right, VelocityVectorColor);
        DebugDraw.DrawLine(velocityEnd, velocityEnd - velocityNormalized * arrowSize - right, VelocityVectorColor);
        DebugDraw.DrawLine(velocityEnd, velocityEnd - velocityNormalized * arrowSize + up, VelocityVectorColor);
        DebugDraw.DrawLine(velocityEnd, velocityEnd - velocityNormalized * arrowSize - up, VelocityVectorColor);
    }

    private static Color GetBodyColor(BodyType bodyType, bool isEnabled)
    {
        if (!isEnabled) return SleepingBodyColor;

        return bodyType switch
        {
            BodyType.Static => StaticBodyColor,
            BodyType.Kinematic => KinematicBodyColor,
            BodyType.Dynamic => DynamicBodyColor,
            _ => Color.White
        };
    }

    /// <summary>
    /// Draw contact points between physics bodies (placeholder for future BepuPhysics callback integration)
    /// </summary>
    public static void DrawContactPointsAtPositions(IEnumerable<Vector3> contactPoints)
    {
        if (!DebugDraw.IsEnabled || !DrawContactPoints) return;

        foreach (var point in contactPoints)
        {
            // Draw contact point as a small cross
            var size = 0.05f;
            DebugDraw.DrawLine(point - Vector3.UnitX * size, point + Vector3.UnitX * size, ContactPointColor);
            DebugDraw.DrawLine(point - Vector3.UnitY * size, point + Vector3.UnitY * size, ContactPointColor);
            DebugDraw.DrawLine(point - Vector3.UnitZ * size, point + Vector3.UnitZ * size, ContactPointColor);
        }
    }

    /// <summary>
    /// Toggle physics debug visualization settings
    /// </summary>
    public static void TogglePhysicsDebug()
    {
        DrawPhysicsBodies = !DrawPhysicsBodies;
    }

    public static void ToggleVelocityVectors()
    {
        DrawVelocityVectors = !DrawVelocityVectors;
    }

    public static void ToggleContactPoints()
    {
        DrawContactPoints = !DrawContactPoints;
    }
}