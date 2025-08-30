using System;
using Microsoft.Xna.Framework;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Gameplay.Enemies;

namespace TopDownShooter.Game.Gameplay.Combat;

public class Hitbox : Entity, TopDownShooter.Game.Framework.Components.IUpdateable
{
    public Vector3 Position { get; set; }
    public float Radius { get; set; } = 1f;
    public float Damage { get; set; } = 10f;
    public float Duration { get; set; } = 0.1f;
    
    private float _timeAlive = 0f;
    private bool _hasHit = false;
    
    public override void Update()
    {
        base.Update();
        
        _timeAlive += Time.Delta;
        
        if (!_hasHit)
            CheckForHits();
        
        if (_timeAlive >= Duration)
        {
            Scene?.RemoveEntity(this);
        }
    }
    
    private void CheckForHits()
    {
        var enemies = Scene?.FindEntities<DummyChaser>();
        if (enemies == null) return;
        
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;
            
            var distance = Vector3.Distance(Position, enemy.GetHurtboxCenter());
            if (distance <= Radius + enemy.GetHurtboxRadius())
            {
                enemy.TakeDamage(Damage);
                _hasHit = true;
                break;
            }
        }
    }
}

public class DamageText : Entity, TopDownShooter.Game.Framework.Components.IUpdateable
{
    private Vector3 _startPosition;
    private Vector3 _velocity;
    private float _timeAlive;
    private string _text;
    private const float LIFETIME = 1f;
    private const float RISE_SPEED = 2f;
    
    public DamageText(Vector3 position, string text)
    {
        _startPosition = position;
        Transform.Position = position;
        _text = text;
        _velocity = Vector3.Up * RISE_SPEED;
    }
    
    public override void Update()
    {
        base.Update();
        
        _timeAlive += Time.Delta;
        Transform.Position += _velocity * Time.Delta;
        
        // Slow down over time
        _velocity *= 0.98f;
        
        if (_timeAlive >= LIFETIME)
        {
            Scene?.RemoveEntity(this);
        }
    }
    
    public string GetText() => _text;
    public float GetAlpha() => Math.Max(0f, 1f - (_timeAlive / LIFETIME));
}