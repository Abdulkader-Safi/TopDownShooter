using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Services;
using TopDownShooter.Game.World;

namespace TopDownShooter.Game.Core;

public class GameRoot : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SceneManager _sceneManager;
    private readonly GameManager _gameManager;

    public static GameRoot Instance { get; private set; }
    public static InputService Input { get; private set; }
    public static AudioService Audio { get; private set; }
    public static AssetService Assets { get; private set; }

    public SceneManager SceneManager => _sceneManager;

    public GameRoot()
    {
        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Fixed timestep 60 FPS
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

        // Initialize GameManager first
        _gameManager = new GameManager();

        // Setup window and graphics based on GameManager settings
        Window.Title = _gameManager.WindowTitle;
        _graphics.PreferredBackBufferWidth = _gameManager.WindowWidth;
        _graphics.PreferredBackBufferHeight = _gameManager.WindowHeight;
        _graphics.IsFullScreen = _gameManager.IsFullscreen;
        _graphics.SynchronizeWithVerticalRetrace = _gameManager.VSync;

        // Subscribe to GameManager events
        _gameManager.ResolutionChanged += OnResolutionChanged;
        _gameManager.FullscreenChanged += OnFullscreenChanged;
        _gameManager.WindowTitleChanged += OnWindowTitleChanged;
    }

    protected override void Initialize()
    {
        Input = new InputService();
        Audio = new AudioService();
        Assets = new AssetService();

        _sceneManager = new SceneManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Assets.Initialize(Content, GraphicsDevice);
        LevelManager.Instance.LoadFirstLevel();
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);

        if (Input.IsKeyPressed(Keys.Escape))
            Exit();

        Input.Update();
        _gameManager.Update();
        _sceneManager.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _sceneManager.Draw();

        base.Draw(gameTime);
    }

    private void OnResolutionChanged(int width, int height)
    {
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        _graphics.IsFullScreen = isFullscreen;
        _graphics.ApplyChanges();
    }

    private void OnWindowTitleChanged(string title)
    {
        Window.Title = title;
    }
}