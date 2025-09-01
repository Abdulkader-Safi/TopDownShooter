using Microsoft.Xna.Framework;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Gameplay.Player;
using TopDownShooter.Game.Physics;

namespace TopDownShooter.Game.World;

public enum LevelTransitionDirection
{
    Next,
    Previous
}

public class LevelTransitionTrigger : Entity, Framework.Components.IUpdateable
{
    private readonly AabbCollider _triggerCollider;
    private readonly CollisionWorld _collisionWorld;
    private readonly LevelTransitionDirection _direction;
    private bool _hasTriggered;

    public LevelTransitionTrigger(CollisionWorld collisionWorld, Vector3 position, Vector3 size, LevelTransitionDirection direction = LevelTransitionDirection.Next)
    {
        _collisionWorld = collisionWorld;
        _direction = direction;
        _hasTriggered = false;

        // Set up visual representation (green box)
        Transform.Position = position;
        var renderer = AddComponent<ModelRenderer>();
        renderer.Color = Color.Green;
        renderer.Size = size;

        // Set up trigger collider
        _triggerCollider = new AabbCollider
        {
            Position = position,
            Size = size
        };
        _collisionWorld.AddStatic(_triggerCollider);
    }

    public override void Update()
    {
        if (_hasTriggered) return;

        // Check for collision with player
        var playerEntities = Scene.FindEntities<PlayerController>();
        foreach (var player in playerEntities)
        {
            // Calculate distance for proximity check
            var distance = Vector3.Distance(player.Transform.Position, Transform.Position);

            // Debug: print when player is close

            if (distance < 2.0f) // Trigger when player is within 2 units
            {
                System.Console.WriteLine($"Trigger activated! Direction: {_direction}, Loading level...");
                _hasTriggered = true;
                TriggerLevelTransition();
                break;
            }
        }
    }

    private void TriggerLevelTransition()
    {
        if (_direction == LevelTransitionDirection.Next)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            LevelManager.Instance.LoadPreviousLevel();
        }
    }
}