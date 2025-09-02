using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GameSystems;
using Core.Framework;
using Core.Framework.Components;
using Game.Gameplay.Combat;
using Game.Gameplay.Enemies;
using Game.Gameplay.Player;
using Core.Physics;
using Core.Rendering;
using Game.World;

namespace Game.Scenes;

public class Level2Scene : Scene
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

        // Generate different world layout for level 2
        GenerateLevel2Arena();

        // Create player
        _player = new PlayerController();
        // Start player at different position for level 2
        _player.Transform.Position = new Vector3(-12, 2f, -12);
        AddEntity(_player);

        // Spawn different enemy configuration
        SpawnLevel2Enemies();

        // Initialize shared rendering resources
        ModelRenderer.InitializeSharedResources(GameRoot.Instance.GraphicsDevice);
        DebugDraw.Initialize(GameRoot.Instance.GraphicsDevice);
    }

    private void GenerateLevel2Arena()
    {
        // Create outer walls - larger arena with dramatic purple theme
        CreateWall(new Vector3(0, 3f, 20), new Vector3(40, 6, 2), Color.DarkMagenta); // North - taller and thicker
        CreateWall(new Vector3(0, 3f, -20), new Vector3(40, 6, 2), Color.DarkMagenta); // South  
        CreateWall(new Vector3(20, 3f, 0), new Vector3(2, 6, 40), Color.DarkMagenta); // East
        CreateWall(new Vector3(-20, 3f, 0), new Vector3(2, 6, 40), Color.DarkMagenta); // West

        // Create dramatic pillar formations
        CreateWall(new Vector3(0, 4f, 0), new Vector3(3, 8, 3), Color.Purple); // Central tall pillar
        CreateWall(new Vector3(-10, 2f, 10), new Vector3(4, 4, 4), Color.MediumPurple);
        CreateWall(new Vector3(10, 2f, -10), new Vector3(4, 4, 4), Color.MediumPurple);
        CreateWall(new Vector3(-10, 3f, -10), new Vector3(2, 6, 6), Color.BlueViolet);
        CreateWall(new Vector3(10, 3f, 10), new Vector3(2, 6, 6), Color.BlueViolet);

        // Create stepped platform structures
        CreateWall(new Vector3(-15, 1f, 0), new Vector3(6, 2, 6), Color.Indigo);
        CreateWall(new Vector3(15, 1f, 0), new Vector3(6, 2, 6), Color.Indigo);
        CreateWall(new Vector3(0, 1f, 15), new Vector3(8, 2, 4), Color.DarkViolet);
        CreateWall(new Vector3(0, 1f, -15), new Vector3(8, 2, 4), Color.DarkViolet);

        // Add some unique geometric obstacles
        CreateWall(new Vector3(-5, 1.5f, 5), new Vector3(1, 3, 8), Color.Magenta); // Tall thin walls
        CreateWall(new Vector3(5, 1.5f, -5), new Vector3(1, 3, 8), Color.Magenta);
        CreateWall(new Vector3(-5, 2.5f, -5), new Vector3(6, 1, 1), Color.Cyan); // Floating beams
        CreateWall(new Vector3(5, 2.5f, 5), new Vector3(6, 1, 1), Color.Cyan);

        // Create glowing level transition trigger (bright green to stand out against purple)
        // This trigger goes back to the previous level (Level 1)
        // Position it in a more accessible location away from walls
        var levelTrigger = new LevelTransitionTrigger(_collisionWorld, new Vector3(12, 1, 12), new Vector3(2.5f, 2.5f, 2.5f), LevelTransitionDirection.Previous);
        AddEntity(levelTrigger);

        // Create main floor with collision
        CreateWall(new Vector3(0, 0f, 0), new Vector3(40, 0.1f, 40), Color.MidnightBlue);

        // Add decorative floor patterns
        CreateFloorDecoration(new Vector3(0, 0.05f, 0), new Vector3(20, 0.05f, 20), Color.DarkSlateBlue);
        CreateFloorDecoration(new Vector3(0, 0.1f, 0), new Vector3(10, 0.05f, 10), Color.SlateBlue);

        // Corner accent floors
        CreateFloorDecoration(new Vector3(-15, 0.05f, -15), new Vector3(6, 0.05f, 6), Color.Purple);
        CreateFloorDecoration(new Vector3(15, 0.05f, 15), new Vector3(6, 0.05f, 6), Color.Purple);
        CreateFloorDecoration(new Vector3(-15, 0.05f, 15), new Vector3(6, 0.05f, 6), Color.Purple);
        CreateFloorDecoration(new Vector3(15, 0.05f, -15), new Vector3(6, 0.05f, 6), Color.Purple);
    }

    private void CreateFloorDecoration(Vector3 position, Vector3 size, Color color)
    {
        var decoration = new Entity();
        decoration.Transform.Position = position;
        var renderer = decoration.AddComponent<ModelRenderer>();
        renderer.Color = color;
        renderer.Size = size;
        AddEntity(decoration);
    }

    private void CreateWall(Vector3 position, Vector3 size, Color color)
    {
        // Visual representation
        var wall = new Entity();
        wall.Transform.Position = position;
        var renderer = wall.AddComponent<ModelRenderer>();
        renderer.Color = color;
        renderer.Size = size;
        AddEntity(wall);

        // Physics representation
        var collider = new AabbCollider
        {
            Position = position,
            Size = size
        };
        _collisionWorld.AddStatic(collider);
    }

    // Overload for backward compatibility
    private void CreateWall(Vector3 position, Vector3 size)
    {
        CreateWall(position, size, Color.DarkRed);
    }

    private void SpawnLevel2Enemies()
    {
        // More challenging enemy placement for level 2
        var spawnPositions = new Vector3[]
        {
            new Vector3(15, 2, 10),
            new Vector3(-15, 2, 10),
            new Vector3(15, 2, -10),
            new Vector3(-15, 2, -10),
            new Vector3(0, 2, 18),
            new Vector3(0, 2, -18),
            new Vector3(18, 2, 0),
            new Vector3(-18, 2, 0)
        };

        foreach (var pos in spawnPositions)
        {
            var enemy = new DummyChaser();
            enemy.Transform.Position = pos;
            AddEntity(enemy);
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();

        try
        {
            _font = GameRoot.Assets.LoadContent<SpriteFont>("DefaultFont");
        }
        catch
        {
            _font = null;
        }
    }

    public override void Update()
    {
        base.Update();

        // Update camera to follow player
        if (_player != null)
        {
            _camera.SetTopDownFollow(_player.Transform.Position, GameManager.Instance.CameraDistance, GameManager.Instance.CameraTilt);
        }
    }

    public override void Draw()
    {
        var device = GameRoot.Instance.GraphicsDevice;

        // Setup 3D rendering state
        device.DepthStencilState = DepthStencilState.Default;
        device.RasterizerState = RasterizerState.CullNone; // Disable culling to fix inverted boxes
        device.BlendState = BlendState.Opaque;

        // Draw 3D entities
        base.Draw();

        // Draw collision boxes if enabled
        if (GameManager.Instance.ShowCollisionBoxes)
        {
            DrawCollisionBoxes();
        }

        // Draw debug visualization if enabled
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
        DebugDraw.IsEnabled = true;

        // Draw static collision boxes in bright colors
        foreach (var collider in _collisionWorld.GetAllStatic())
        {
            if (collider is AabbCollider aabb)
            {
                DebugDraw.DrawBox(aabb.GetBounds(), Color.Red);
            }
        }

        // Draw dynamic collision boxes
        foreach (var collider in _collisionWorld.GetAllDynamic())
        {
            if (collider is CapsuleCollider capsule)
            {
                DebugDraw.DrawCapsule(capsule.Position, capsule.Radius, capsule.HalfHeight, Color.Yellow);
            }
        }

        // Draw player collision capsule
        if (_player != null)
        {
            var motor = _player.GetComponent<CharacterMotor>();
            if (motor != null)
            {
                DebugDraw.DrawCapsuleOutline(_player.Transform.Position, motor.Radius, motor.Height * 0.5f, Color.Cyan, 0.08f);
                DebugDraw.DrawCapsule(_player.Transform.Position, motor.Radius, motor.Height * 0.5f, Color.Blue);
            }
        }
    }

    private void DrawDebugInfo()
    {
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

        // Prominent level indicator
        if (_font != null)
        {
            var viewport = GameRoot.Instance.GraphicsDevice.Viewport;
            var levelText = "LEVEL 2 - PURPLE REALM";
            var levelTextSize = _font.MeasureString(levelText);
            var levelTextPos = new Vector2(
                (viewport.Width - levelTextSize.X) / 2, // Center horizontally
                20 // Top of screen
            );

            // Draw shadow for better visibility
            _spriteBatch.DrawString(_font, levelText, levelTextPos + Vector2.One * 2, Color.Black);
            // Draw main text
            _spriteBatch.DrawString(_font, levelText, levelTextPos, Color.Magenta);

            // Also show in bottom left
            _spriteBatch.DrawString(_font, "LEVEL 2", new Vector2(10, viewport.Height - 30), Color.Cyan);
        }

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

            // Show player status
            if (_player != null)
            {
                var groundedText = _player.Motor.IsGrounded ? "Grounded" : "Airborne";
                _spriteBatch.DrawString(_font, $"Player: {groundedText}", new Vector2(10, yOffset + 30), _player.Motor.IsGrounded ? Color.Green : Color.Orange);
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