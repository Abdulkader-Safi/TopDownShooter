using System;
using Microsoft.Xna.Framework;
using Core.GameSystems;
using Core.Physics;

namespace Core.Framework.Components;

public class CharacterMotor : IComponent, IUpdateable
{
    public Entity Entity { get; set; }

    public float Radius { get; set; } = 0.5f;
    public float Height { get; set; } = 1.8f;
    private float GroundSnapDistance { get; set; } = 0.1f;
    public bool IsGrounded { get; private set; }

    // Gravity settings
    private float Gravity { get; set; } = 20f; // Units per second squared
    private float TerminalVelocity { get; set; } = 30f; // Maximum fall speed

    private Vector3 _verticalVelocity; // Separate vertical velocity for gravity
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

        // Apply gravity to vertical velocity
        if (!IsGrounded)
        {
            _verticalVelocity.Y -= Gravity * deltaTime;
            // Clamp to terminal velocity
            if (_verticalVelocity.Y < -TerminalVelocity)
                _verticalVelocity.Y = -TerminalVelocity;
        }
        else
        {
            // Reset vertical velocity when grounded
            _verticalVelocity.Y = 0f;
        }

        // Separate horizontal and vertical movement to avoid floor collision issues
        var horizontalVelocity = new Vector3(desiredVelocity.X, 0, desiredVelocity.Z);
        var horizontalMovement = horizontalVelocity * deltaTime;
        var verticalMovement = new Vector3(0, _verticalVelocity.Y * deltaTime, 0);

        var originalPosition = Entity.Transform.Position;

        // First, perform horizontal movement
        var afterHorizontal = SweepMove(originalPosition, horizontalMovement, 3);

        // Then, perform vertical movement separately
        var newPosition = SweepMove(afterHorizontal, verticalMovement, 3);

        // Ground detection - check if we're standing on something
        CheckGrounded(newPosition);

        Entity.Transform.Position = newPosition;
        _collider.Position = newPosition;
    }

    public void Jump(float jumpForce)
    {
        if (IsGrounded)
        {
            _verticalVelocity.Y = jumpForce;
            IsGrounded = false;
        }
    }

    private void CheckGrounded(Vector3 position)
    {
        // Check directly downward from character center for ground
        var groundCheckStart = position;
        var groundCheckEnd = position - Vector3.UnitY * (Height * 0.5f + GroundSnapDistance + 0.1f);

        // Use a smaller capsule for ground checking to avoid side collisions
        if (CollisionWorld.Instance.CastCapsule(groundCheckStart, groundCheckEnd, Radius * 0.8f, Height * 0.4f, out var groundHit))
        {
            // Check if we're actually above the ground (not hitting a wall)
            var hitNormal = Vector3.UnitY; // Assume upward normal for floors
            var distanceToGround = groundCheckStart.Y - groundHit.Position.Y;

            // Only consider it ground if we're close and above it
            if (distanceToGround >= 0 && distanceToGround <= Height * 0.5f + 0.2f)
            {
                // If we're falling or on ground, snap to ground level
                if (_verticalVelocity.Y <= 0.1f)
                {
                    var groundY = groundHit.Position.Y + (Height * 0.5f) + 0.05f; // Slight offset above ground
                    Entity.Transform.Position = new Vector3(position.X, groundY, position.Z);
                    _verticalVelocity.Y = 0f;
                    IsGrounded = true;
                    return;
                }
            }
        }

        // Not grounded
        IsGrounded = false;
    }

    private Vector3 SweepMove(Vector3 startPos, Vector3 movement, int maxIterations)
    {
        var currentPos = startPos;
        var remainingMovement = movement;

        for (var i = 0; i < maxIterations && remainingMovement.LengthSquared() > 0.0001f; i++)
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