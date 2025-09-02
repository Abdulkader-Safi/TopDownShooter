using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core.Framework;
using Core.Services;
using Game.Scenes;

namespace Core.GameSystems;

public class GameRoot : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SceneManager _sceneManager;
    private readonly GameManager _gameManager;
    private readonly PerformanceTracker _performanceTracker;

    public static GameRoot Instance { get; private set; }
    public static InputService Input { get; private set; }
    public static AudioService Audio { get; private set; }
    public static AssetService Assets { get; private set; }
    public static MyraService Myra { get; private set; }
    public static PhysicsService Physics { get; private set; }

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

        // Initialize GameManager and PerformanceTracker
        _gameManager = new GameManager();
        _performanceTracker = new PerformanceTracker();

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
        Myra = new MyraService();
        Physics = new PhysicsService();

        _sceneManager = new SceneManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Assets.Initialize(Content, GraphicsDevice);
        Myra.Initialize(GraphicsDevice, this, _performanceTracker);
        Physics.Initialize();

        // Start with the main menu instead of directly loading the first level
        var startMenu = new StartMenuScene();
        _sceneManager.LoadScene(startMenu);
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);
        _performanceTracker.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        if (Input.IsKeyPressed(Keys.Escape))
            Exit();

        Input.Update();
        _gameManager.Update();
        Physics.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        Myra.Update(gameTime);
        _sceneManager.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _sceneManager.Draw();

        // Draw Myra UI (includes FPS chart)
        Myra.Draw();

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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Physics?.Shutdown();
        }

        base.Dispose(disposing);
    }
}