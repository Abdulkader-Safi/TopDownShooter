using System;
using Microsoft.Xna.Framework;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Physics;

namespace TopDownShooter.Game.Framework.Components;

public class CharacterMotor : IComponent, IUpdateable
{
    public Entity Entity { get; set; }

    public float Radius { get; set; } = 0.5f;
    public float Height { get; set; } = 1.8f;
    public float GroundSnapDistance { get; set; } = 0.1f;
    public bool IsGrounded { get; private set; }

    private Vector3 _velocity;
    private CapsuleCollider _collider;

    public void Initialize()
    {
        _collider = new CapsuleCollider
        {
            Position = Entity.Transform.Position,
            Radius = Radius,
            HalfHeight = Height * 0.5f
        };
    }

    public void Move(Vector3 desiredVelocity, float deltaTime)
    {
        if (_collider == null) Initialize();

        _velocity = desiredVelocity;
        var movement = _velocity * deltaTime;
        var originalPosition = Entity.Transform.Position;

        // Perform swept movement with collision resolution
        var newPosition = SweepMove(originalPosition, movement, 3);

        // Simple ground snapping - only if we didn't move much horizontally
        var horizontalMovement = new Vector3(newPosition.X - originalPosition.X, 0, newPosition.Z - originalPosition.Z);
        if (horizontalMovement.LengthSquared() < 0.1f) // Only snap if we're not sliding along walls
        {
            var groundCheckStart = newPosition + Vector3.UnitY * 0.1f;
            var groundCheckEnd = newPosition - Vector3.UnitY * (GroundSnapDistance + 0.1f);

            if (CollisionWorld.Instance.CastCapsule(groundCheckStart, groundCheckEnd, Radius, Height * 0.5f, out var groundHit))
            {
                newPosition.Y = groundHit.Position.Y + (Height * 0.5f);
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }
        else
        {
            // Assume grounded if we're moving horizontally (sliding along walls)
            IsGrounded = true;
        }

        Entity.Transform.Position = newPosition;
        _collider.Position = newPosition;
    }

    private Vector3 SweepMove(Vector3 startPos, Vector3 movement, int maxIterations)
    {
        var currentPos = startPos;
        var remainingMovement = movement;

        for (int i = 0; i < maxIterations && remainingMovement.LengthSquared() > 0.0001f; i++)
        {
            var targetPos = currentPos + remainingMovement;

            if (CollisionWorld.Instance.CastCapsule(currentPos, targetPos, Radius, Height * 0.5f, out var hit))
            {
                // Move to just before contact point (minimal back-off)
                if (hit.Distance > 0.002f)
                {
                    var moveDistance = hit.Distance - 0.002f; // Minimal back-off
                    currentPos += Vector3.Normalize(remainingMovement) * Math.Max(0, moveDistance);
                }

                // Calculate remaining movement after contact
                var usedDistance = Math.Max(0, hit.Distance - 0.002f);
                var remainingDistance = remainingMovement.Length() - usedDistance;

                if (remainingDistance > 0.001f)
                {
                    // Slide along surface - project remaining movement onto the surface
                    remainingMovement = remainingMovement.ProjectOnPlane(hit.Normal);
                    if (remainingMovement.LengthSquared() > 0.0001f)
                    {
                        remainingMovement = Vector3.Normalize(remainingMovement) * remainingDistance;
                    }
                    else
                    {
                        remainingMovement = Vector3.Zero; // Can't slide further
                    }
                }
                else
                {
                    remainingMovement = Vector3.Zero;
                }
            }
            else
            {
                // No collision, move freely
                currentPos = targetPos;
                break;
            }
        }

        return currentPos;
    }

    public void Update()
    {
        if (_collider != null)
            _collider.Position = Entity.Transform.Position;
    }
}