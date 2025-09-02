using Microsoft.Xna.Framework;
using Core.GameSystems;
using Core.Physics;

namespace Core.Framework.Components;

public enum BodyType
{
    Static,
    Kinematic,
    Dynamic
}

public class RigidBodyComponent : IComponent, IUpdateable
{
    public Entity Entity { get; set; }

    public BodyType BodyType { get; set; } = BodyType.Dynamic;
    public float Mass { get; set; } = 1.0f;
    public bool IsEnabled { get; set; } = true;

    private int _bodyHandle = -1;
    private bool _isInitialized = false;
    private object _collidableDescription;

    public int BodyHandle => _bodyHandle;
    public bool IsInitialized => _isInitialized;

    public Vector3 Velocity
    {
        get => IsInitialized ? GameRoot.Physics.PhysicsWorld.GetBodyVelocity(_bodyHandle) : Vector3.Zero;
        set { if (IsInitialized) GameRoot.Physics.PhysicsWorld.SetBodyVelocity(_bodyHandle, value); }
    }

    public Vector3 Position
    {
        get => IsInitialized ? GameRoot.Physics.PhysicsWorld.GetBodyPosition(_bodyHandle) : Entity.Transform.Position;
        set
        {
            if (IsInitialized)
                GameRoot.Physics.PhysicsWorld.SetBodyPosition(_bodyHandle, value);
            Entity.Transform.Position = value;
        }
    }

    public void Initialize(object collidableDescription)
    {
        if (_isInitialized) return;

        _collidableDescription = collidableDescription;

        var physics = GameRoot.Physics.PhysicsWorld;
        var position = Entity.Transform.Position;
        var orientation = System.Numerics.Quaternion.Identity;

        _bodyHandle = BodyType switch
        {
            BodyType.Static => physics.AddStaticBody(position, orientation, collidableDescription),
            BodyType.Kinematic => physics.AddKinematicBody(position, orientation, collidableDescription),
            BodyType.Dynamic => physics.AddDynamicBody(position, orientation, collidableDescription, Mass),
            _ => throw new System.ArgumentOutOfRangeException()
        };

        physics.RegisterEntityBody(Entity, _bodyHandle);
        _isInitialized = true;
    }

    public void Update()
    {
        if (!IsInitialized || !IsEnabled) return;

        // Sync physics body position back to transform
        if (BodyType == BodyType.Dynamic || BodyType == BodyType.Kinematic)
        {
            Entity.Transform.Position = GameRoot.Physics.PhysicsWorld.GetBodyPosition(_bodyHandle);
        }
    }

    public void ApplyForce(Vector3 force)
    {
        if (IsInitialized && BodyType == BodyType.Dynamic)
        {
            var currentVelocity = GameRoot.Physics.PhysicsWorld.GetBodyVelocity(_bodyHandle);
            GameRoot.Physics.PhysicsWorld.SetBodyVelocity(_bodyHandle, currentVelocity + force / Mass);
        }
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        if (IsInitialized && BodyType == BodyType.Dynamic)
        {
            var currentVelocity = GameRoot.Physics.PhysicsWorld.GetBodyVelocity(_bodyHandle);
            GameRoot.Physics.PhysicsWorld.SetBodyVelocity(_bodyHandle, currentVelocity + impulse);
        }
    }

    public void Destroy()
    {
        if (_isInitialized)
        {
            if (BodyType == BodyType.Static)
                GameRoot.Physics.PhysicsWorld.RemoveStatic(_bodyHandle);
            else
                GameRoot.Physics.PhysicsWorld.RemoveBody(_bodyHandle);

            _isInitialized = false;
            _bodyHandle = -1;
        }
    }
}