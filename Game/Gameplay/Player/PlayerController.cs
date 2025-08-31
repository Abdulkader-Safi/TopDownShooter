using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Gameplay.Combat;
using TopDownShooter.Game.Rendering;
using TopDownShooter.Game.Services;

namespace TopDownShooter.Game.Gameplay.Player;

public class PlayerController : Entity, TopDownShooter.Game.Framework.Components.IUpdateable
{
    private readonly CharacterMotor _motor;
    private readonly ModelRenderer _renderer;

    private float _dashCooldown = 0f;
    // Movement settings now come from GameManager

    public Vector3 AimTarget { get; private set; }

    public float DashCooldownRatio => MathHelper.Max(0f, _dashCooldown / GameManager.Instance.PlayerDashCooldown);

    public PlayerController()
    {
        _motor = AddComponent<CharacterMotor>();
        _motor.Initialize();

        _renderer = AddComponent<ModelRenderer>();
        _renderer.Color = Color.Blue;
        _renderer.Size = new Vector3(1f, 1.8f, 1f);

        Transform.Position = new Vector3(0, 1f, 0);
    }

    public override void Update()
    {
        base.Update();

        HandleInput();
        UpdateAiming();
        UpdateCooldowns();
    }

    private void HandleInput()
    {
        var input = GameRoot.Input;
        var moveInput = Vector2.Zero;

        // Movement input
        if (input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up))
            moveInput.Y += 1f;
        if (input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
            moveInput.Y -= 1f;
        if (input.IsKeyDown(Keys.A) || input.IsKeyDown(Keys.Left))
            moveInput.X += 1f;
        if (input.IsKeyDown(Keys.D) || input.IsKeyDown(Keys.Right))
            moveInput.X -= 1f;

        // Convert 2D input to 3D world movement (XZ plane) - correct mapping for top-down view
        var worldMovement = new Vector3(moveInput.X, 0, moveInput.Y);
        if (worldMovement.LengthSquared() > 0)
            worldMovement = Vector3.Normalize(worldMovement) * GameManager.Instance.PlayerMoveSpeed;

        // Dash input
        if (input.IsMouseButtonPressed(MouseButton.Right) && _dashCooldown <= 0f)
        {
            var dashDirection = worldMovement.LengthSquared() > 0 ?
                Vector3.Normalize(worldMovement) :
                Vector3.Normalize(AimTarget - Transform.Position).Xz();

            worldMovement = dashDirection * GameManager.Instance.PlayerDashForce;
            _dashCooldown = GameManager.Instance.PlayerDashCooldown;
        }

        _motor.Move(worldMovement, Time.Delta);

        // Attack input
        if (input.IsMouseButtonPressed(MouseButton.Left))
        {
            PerformAttack();
        }
    }

    private void UpdateAiming()
    {
        var camera = Camera.Current;
        if (camera == null) return;

        var mousePos = GameRoot.Input.MousePosition;
        var ray = camera.ScreenToRay(mousePos);

        if (ray.RayPlaneY0(out var hitPoint))
        {
            AimTarget = hitPoint;

            // Face the aim target
            var direction = (AimTarget - Transform.Position).Xz();
            if (direction.LengthSquared() > 0.001f)
            {
                var angle = (float)Math.Atan2(direction.X, direction.Z);
                Transform.Rotation = new Vector3(0, angle, 0);
            }
        }
    }

    private void UpdateCooldowns()
    {
        if (_dashCooldown > 0f)
            _dashCooldown -= Time.Delta;
    }

    private void PerformAttack()
    {
        var hitbox = new Hitbox
        {
            Position = Transform.Position + Transform.Forward * 1.5f,
            Radius = 2f,
            Damage = 25,
            Duration = 0.12f
        };

        Scene.AddEntity(hitbox);
    }
}