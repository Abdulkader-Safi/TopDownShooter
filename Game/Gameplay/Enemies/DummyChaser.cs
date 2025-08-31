using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Gameplay.Combat;
using TopDownShooter.Game.Gameplay.Player;
using TopDownShooter.Game.Physics;

namespace TopDownShooter.Game.Gameplay.Enemies;

public class DummyChaser : Entity, TopDownShooter.Game.Framework.Components.IUpdateable
{
    private readonly CharacterMotor _motor;
    private readonly ModelRenderer _renderer;
    private float _health = 100f;
    private float _flashTimer = 0f;
    private const float MoveSpeed = 2.5f;
    private const float FlashDuration = 0.2f;
    
    public float Health => _health;
    public bool IsDead => _health <= 0f;
    
    public DummyChaser()
    {
        _motor = AddComponent<CharacterMotor>();
        _motor.Initialize();
        
        _renderer = AddComponent<ModelRenderer>();
        _renderer.Color = Color.Red;
        _renderer.Size = new Vector3(1f, 1f, 1f);
    }
    
    public override void Update()
    {
        base.Update();
        
        if (IsDead) return;
        
        UpdateMovement();
        UpdateVisuals();
        
        if (_health <= 0f)
        {
            Scene?.RemoveEntity(this);
        }
    }
    
    private void UpdateMovement()
    {
        var player = Scene?.FindEntity<PlayerController>();
        if (player == null) return;
        
        var toPlayer = player.Transform.Position - Transform.Position;
        toPlayer.Y = 0; // Keep movement on XZ plane

        if (!(toPlayer.LengthSquared() > 0.1f)) return;
        var direction = Vector3.Normalize(toPlayer);
        var desiredVelocity = direction * MoveSpeed;
            
        _motor.Move(desiredVelocity, Time.Delta);
            
        // Face the player
        var angle = (float)Math.Atan2(direction.X, -direction.Z);
        Transform.Rotation = new Vector3(0, angle, 0);
    }
    
    private void UpdateVisuals()
    {
        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.Delta;
            _renderer.Color = Color.White;
        }
        else
        {
            _renderer.Color = Color.Red;
        }
    }
    
    public void TakeDamage(float damage)
    {
        _health -= damage;
        _flashTimer = FlashDuration;
        
        // Create damage text
        var damageText = new DamageText(Transform.Position + Vector3.Up * 2f, damage.ToString(CultureInfo.InvariantCulture));
        Scene?.AddEntity(damageText);
    }
    
    public Vector3 GetHurtboxCenter()
    {
        return Transform.Position + Vector3.Up * 0.5f;
    }
    
    public float GetHurtboxRadius()
    {
        return 0.7f;
    }
}