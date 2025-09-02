using Microsoft.Xna.Framework;
using TopDownShooter.Game.Physics;
using TopDownShooter.Game.Framework;

namespace TopDownShooter.Game.Services;

public class PhysicsService
{
    private SimpleBepuPhysicsWorld _physicsWorld;
    private CollisionWorld _collisionWorld;
    private PhysicsCollisionBridge _collisionBridge;
    
    public SimpleBepuPhysicsWorld PhysicsWorld => _physicsWorld;
    public CollisionWorld CollisionWorld => _collisionWorld;
    public PhysicsCollisionBridge CollisionBridge => _collisionBridge;
    
    public void Initialize()
    {
        _physicsWorld = new SimpleBepuPhysicsWorld();
        _collisionWorld = new CollisionWorld();
        _collisionBridge = new PhysicsCollisionBridge();
    }
    
    public void Update(float deltaTime)
    {
        _physicsWorld?.Update(deltaTime);
        _collisionBridge?.UpdatePhysicsColliders();
    }
    
    public void RegisterEntity(Entity entity)
    {
        _collisionBridge?.RegisterPhysicsEntity(entity);
    }
    
    public void UnregisterEntity(Entity entity)
    {
        _collisionBridge?.UnregisterPhysicsEntity(entity);
    }
    
    public void Shutdown()
    {
        _physicsWorld?.Dispose();
        _physicsWorld = null;
        _collisionBridge = null;
    }
}