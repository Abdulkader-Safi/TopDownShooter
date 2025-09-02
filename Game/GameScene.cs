using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Framework.Components;
using TopDownShooter.Game.Gameplay.Combat;
using TopDownShooter.Game.Gameplay.Enemies;
using TopDownShooter.Game.Gameplay.Player;
using TopDownShooter.Game.Physics;
using TopDownShooter.Game.Rendering;
using TopDownShooter.Game.World;

namespace TopDownShooter.Game;

public class GameScene : Scene
{
    private Camera _camera;
    private CollisionWorld _collisionWorld;
    private PlayerController _player;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _pixelTexture;

    public override void Initialize()
    {
        base.Initialize();

        // Initialize systems
        _collisionWorld = new CollisionWorld();
        _camera = new Camera(GameRoot.Instance.GraphicsDevice);
        _spriteBatch = new SpriteBatch(GameRoot.Instance.GraphicsDevice);

        // Create a simple pixel texture for UI rendering
        _pixelTexture = new Texture2D(GameRoot.Instance.GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        // Generate world
        Level.GenerateArena(this, _collisionWorld);

        // Create player
        _player = new PlayerController();
        AddEntity(_player);

        // Spawn enemies
        SpawnEnemies();

        // Initialize shared rendering resources
        ModelRenderer.InitializeSharedResources(GameRoot.Instance.GraphicsDevice);
        DebugDraw.Initialize(GameRoot.Instance.GraphicsDevice);
    }

    public override void LoadContent()
    {
        base.LoadContent();

        // Create a basic font (fallback if no font asset is available)
        try
        {
            _font = GameRoot.Assets.LoadContent<SpriteFont>("DefaultFont");
        }
        catch
        {
            // If no font is available, we'll render without text
            _font = null;
        }
    }

    private void SpawnEnemies()
    {
        var spawnPositions = new Vector3[]
        {
            new Vector3(10, 2, 10),
            new Vector3(-10, 2, 10),
            new Vector3(10, 2, -10),
            new Vector3(-10, 2, -10),
            new Vector3(0, 2, 12)
        };

        foreach (var pos in spawnPositions)
        {
            var enemy = new DummyChaser();
            enemy.Transform.Position = pos;
            AddEntity(enemy);
        }
    }

    public override void Update()
    {
        base.Update();

        // Update camera to follow player using GameManager settings
        if (_player != null)
        {
            _camera.SetTopDownFollow(_player.Transform.Position, GameManager.Instance.CameraDistance, GameManager.Instance.CameraTilt);
        }

        // GameManager now handles all debug input toggles automatically
    }

    public override void Draw()
    {
        var device = GameRoot.Instance.GraphicsDevice;

        // Setup 3D rendering state
        device.DepthStencilState = DepthStencilState.Default;
        device.RasterizerState = RasterizerState.CullCounterClockwise;
        device.BlendState = BlendState.Opaque;

        // Draw 3D entities
        base.Draw();

        // Draw collision boxes if enabled in GameManager
        if (GameManager.Instance.ShowCollisionBoxes)
        {
            DrawCollisionBoxes();
        }

        // Draw debug visualization if enabled in GameManager
        if (GameManager.Instance.ShowDebugInfo)
        {
            DrawDebugInfo();
        }

        // Draw debug lines
        DebugDraw.Render(device, _camera.View, _camera.Projection);

        // Draw UI
        DrawUI();
    }

    private void DrawCollisionBoxes()
    {
        // Show collision boxes for obstacles with bright colors
        DebugDraw.IsEnabled = true; // Enable for this frame

        // Draw static collision boxes (walls and obstacles) in bright red
        foreach (var collider in _collisionWorld.GetAllStatic())
        {
            if (collider is AabbCollider aabb)
            {
                DebugDraw.DrawBox(aabb.GetBounds(), Color.Red);
            }
        }

        // Draw dynamic collision boxes (player and enemies) in bright yellow  
        foreach (var collider in _collisionWorld.GetAllDynamic())
        {
            if (collider is CapsuleCollider capsule)
            {
                DebugDraw.DrawCapsule(capsule.Position, capsule.Radius, capsule.HalfHeight, Color.Yellow);
            }
        }

        // Draw player collision capsule with thick outline in bright colors
        if (_player != null)
        {
            var motor = _player.GetComponent<CharacterMotor>();
            if (motor != null)
            {
                // Draw thick outlined capsule for better visibility
                DebugDraw.DrawCapsuleOutline(_player.Transform.Position, motor.Radius, motor.Height * 0.5f, Color.Cyan, 0.08f);
                // Draw inner capsule in contrasting color
                DebugDraw.DrawCapsule(_player.Transform.Position, motor.Radius, motor.Height * 0.5f, Color.Blue);
            }
        }
    }

    private void DrawDebugInfo()
    {
        // Additional debug info when F1 is pressed

        // Draw player aim ray
        if (_player != null)
        {
            DebugDraw.DrawLine(_player.Transform.Position, _player.AimTarget, Color.Cyan);
            DebugDraw.DrawSphere(_player.AimTarget, 0.2f, Color.White);
        }
    }

    private void DrawUI()
    {
        _spriteBatch.Begin();

        // FPS counter
        var fps = (int)(1.0 / Time.Delta);
        var fpsText = $"FPS: {fps}";

        if (_font != null && GameManager.Instance.ShowFps)
        {
            _spriteBatch.DrawString(_font, fpsText, new Vector2(10, 10), Color.White);
        }

        // Show debug UI information
        if (_font != null)
        {
            int yOffset = GameManager.Instance.ShowFps ? 30 : 10;

            // Show collision box toggle status
            var collisionText = GameManager.Instance.ShowCollisionBoxes ? "Collision Boxes: ON (F2)" : "Collision Boxes: OFF (F2)";
            _spriteBatch.DrawString(_font, collisionText, new Vector2(10, yOffset), GameManager.Instance.ShowCollisionBoxes ? Color.Red : Color.Gray);

            // Show debug info toggle status
            var debugText = GameManager.Instance.ShowDebugInfo ? "Debug Info: ON (F1)" : "Debug Info: OFF (F1)";
            _spriteBatch.DrawString(_font, debugText, new Vector2(10, yOffset + 20), GameManager.Instance.ShowDebugInfo ? Color.Cyan : Color.Gray);

            // Show additional controls
            _spriteBatch.DrawString(_font, "SPACE: Jump | F3: Toggle FPS | F4: Performance | F5: FPS Chart | F11: Fullscreen", new Vector2(10, yOffset + 40), Color.Gray);

            // Show player status
            if (_player != null)
            {
                var groundedText = _player.Motor.IsGrounded ? "Grounded" : "Airborne";
                _spriteBatch.DrawString(_font, $"Player: {groundedText}", new Vector2(10, yOffset + 60), _player.Motor.IsGrounded ? Color.Green : Color.Orange);
            }

            // Show performance stats if enabled
            if (GameManager.Instance.ShowPerformanceStats)
            {
                var perfText = GameManager.Instance.GetDebugInfo();
                _spriteBatch.DrawString(_font, perfText, new Vector2(10, yOffset + 70), Color.Yellow);
            }
        }

        // Draw crosshair at aim point
        if (_player != null && _camera != null)
        {
            var screenPos = GameRoot.Instance.GraphicsDevice.Viewport.Project(
                _player.AimTarget, _camera.Projection, _camera.View, Matrix.Identity);

            if (screenPos.Z >= 0 && screenPos.Z <= 1)
            {
                var crosshairPos = new Vector2(screenPos.X, screenPos.Y);
                DrawCrosshair(crosshairPos);
            }
        }

        // Draw dash cooldown bar
        if (_player != null && _player.DashCooldownRatio > 0f)
        {
            var viewport = GameRoot.Instance.GraphicsDevice.Viewport;
            var barWidth = 200;
            var barHeight = 20;
            var barX = (viewport.Width - barWidth) / 2;
            var barY = viewport.Height - 50;

            // Background
            DrawRectangle(_spriteBatch, new Rectangle(barX, barY, barWidth, barHeight), Color.Gray);

            // Cooldown progress
            var progressWidth = (int)(barWidth * (1f - _player.DashCooldownRatio));
            DrawRectangle(_spriteBatch, new Rectangle(barX, barY, progressWidth, barHeight), Color.Blue);
        }

        // Draw damage text
        var damageTexts = FindEntities<DamageText>();
        foreach (var damageText in damageTexts)
        {
            if (_font != null && _camera != null)
            {
                var worldPos = damageText.Transform.Position;
                var screenPos = GameRoot.Instance.GraphicsDevice.Viewport.Project(
                    worldPos, _camera.Projection, _camera.View, Matrix.Identity);

                if (screenPos.Z >= 0 && screenPos.Z <= 1)
                {
                    var alpha = damageText.GetAlpha();
                    var color = Color.White * alpha;
                    var textPos = new Vector2(screenPos.X, screenPos.Y);
                    _spriteBatch.DrawString(_font, damageText.GetText(), textPos, color);
                }
            }
        }

        _spriteBatch.End();

        // Reset blend state for 3D rendering
        GameRoot.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
    }

    private void DrawCrosshair(Vector2 position)
    {
        var size = 10f;
        var thickness = 2f;

        // Draw crosshair lines
        DrawRectangle(_spriteBatch, new Rectangle((int)(position.X - size), (int)(position.Y - thickness / 2), (int)(size * 2), (int)thickness), Color.White);
        DrawRectangle(_spriteBatch, new Rectangle((int)(position.X - thickness / 2), (int)(position.Y - size), (int)thickness, (int)(size * 2)), Color.White);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        spriteBatch.Draw(_pixelTexture, rectangle, color);
    }
}