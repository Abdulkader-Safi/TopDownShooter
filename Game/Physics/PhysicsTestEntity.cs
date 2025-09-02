using System;
using Microsoft.Xna.Framework;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Core;

namespace TopDownShooter.Game.Physics;

/// <summary>
/// Simple test entity to verify physics integration works correctly
/// </summary>
public class PhysicsTestEntity : Entity, TopDownShooter.Game.Framework.Components.IUpdateable
{
    private readonly ModelRenderer _renderer;
    private float _testTimer = 0f;
    
    public PhysicsTestEntity(Vector3 position, BodyType bodyType = BodyType.Dynamic)
    {
        Transform.Position = position;
        
        // Add visual representation
        _renderer = AddComponent<ModelRenderer>();
        _renderer.Color = bodyType switch
        {
            BodyType.Static => Color.Blue,
            BodyType.Kinematic => Color.Orange,
            BodyType.Dynamic => Color.Green,
            _ => Color.White
        };
        _renderer.Size = Vector3.One;
        
        // Create physics body
        switch (bodyType)
        {
            case BodyType.Static:
                CreateStaticBox(Vector3.One);
                break;
            case BodyType.Kinematic:
                CreateKinematicBox(Vector3.One);
                break;
            case BodyType.Dynamic:
                CreateDynamicBox(Vector3.One, 1.0f);
                break;
        }
        
        // Register with physics system
        GameRoot.Physics.RegisterEntity(this);
    }
    
    public override void Update()
    {
        _testTimer += Time.Delta;
        
        // Test different physics behaviors based on body type
        var rigidBody = GetRigidBody();
        if (rigidBody?.IsInitialized == true)
        {
            switch (rigidBody.BodyType)
            {
                case BodyType.Dynamic:
                    TestDynamicBehavior(rigidBody);
                    break;
                case BodyType.Kinematic:
                    TestKinematicBehavior(rigidBody);
                    break;
            }
        }
    }
    
    private void TestDynamicBehavior(RigidBodyComponent rigidBody)
    {
        // Apply periodic impulse to test physics simulation
        if (_testTimer > 2f)
        {
            var impulse = new Vector3(
                (float)(System.Random.Shared.NextDouble() - 0.5) * 10f,
                5f,
                (float)(System.Random.Shared.NextDouble() - 0.5) * 10f
            );
            
            rigidBody.ApplyImpulse(impulse);
            _testTimer = 0f;
        }
    }
    
    private void TestKinematicBehavior(RigidBodyComponent rigidBody)
    {
        // Move in a circular pattern to test kinematic movement
        var radius = 5f;
        var speed = 0.5f;
        var centerX = 10f;
        var centerZ = 0f;
        
        var newX = centerX + (float)Math.Cos(_testTimer * speed) * radius;
        var newZ = centerZ + (float)Math.Sin(_testTimer * speed) * radius;
        
        rigidBody.Position = new Vector3(newX, Transform.Position.Y, newZ);
    }
    
    public static void SpawnTestEntities(Scene scene)
    {
        // Create test entities to verify physics integration
        
        // Static entities (ground/walls)
        var ground = new PhysicsTestEntity(new Vector3(0, -1, 0), BodyType.Static);
        ground._renderer.Size = new Vector3(20, 1, 20);
        ground.GetCollider().Size = new Vector3(20, 1, 20);
        scene.AddEntity(ground);
        
        // Dynamic entities (falling/moving objects)
        for (var i = 0; i < 3; i++)
        {
            var dynamic = new PhysicsTestEntity(
                new Vector3(-5 + i * 2, 10, 0), 
                BodyType.Dynamic
            );
            scene.AddEntity(dynamic);
        }
        
        // Kinematic entity (scripted movement)
        var kinematic = new PhysicsTestEntity(
            new Vector3(10, 2, 0), 
            BodyType.Kinematic
        );
        kinematic._renderer.Color = Color.Orange;
        scene.AddEntity(kinematic);
    }
}