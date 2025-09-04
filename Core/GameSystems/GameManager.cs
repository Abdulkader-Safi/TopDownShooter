using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core.GameSystems;

public class GameManager
{
    public static GameManager Instance { get; private set; }

    // Graphics Settings
    public const GraphicsProfile graphicsProfile = GraphicsProfile.HiDef;
    public int WindowWidth { get; private set; } = 1280;
    public int WindowHeight { get; private set; } = 720;
    public string WindowTitle { get; private set; } = "Learn Monogame 3D";
    public bool IsFullscreen { get; private set; } = false;
    public bool VSync { get; private set; } = true;

    // Debug Settings
    public bool ShowCollisionBoxes { get; private set; } = true;
    public bool ShowDebugInfo { get; private set; } = false;
    public bool ShowFps { get; private set; } = true;
    public bool ShowPlayerOutline { get; set; } = true;
    public bool ShowFpsChart { get; private set; } = true;

    // Gameplay Settings
    public float PlayerMoveSpeed { get; private set; } = 5f;
    public float PlayerDashForce { get; private set; } = 15f;
    public float PlayerDashCooldown { get; private set; } = 1f;
    public float CameraDistance { get; private set; } = 18f;
    public float CameraTilt { get; private set; } = 55f;

    // Audio Settings
    public float MasterVolume { get; set; } = 1f;
    public float SfxVolume { get; set; } = 0.8f;
    public float MusicVolume { get; set; } = 0.6f;

    // Performance Settings
    private int TargetFps { get; set; } = 60;
    public bool ShowPerformanceStats { get; set; } = false;

    // Input Settings for toggling
    private bool _previousF1State = false;
    private bool _previousF2State = false;
    private bool _previousF3State = false;
    private bool _previousF4State = false;
    private bool _previousF5State = false;
    private bool _previousF11State = false;

    // Events
    public event Action<int, int> ResolutionChanged;
    public event Action<bool> FullscreenChanged;
    public event Action<string> WindowTitleChanged;

    public GameManager()
    {
        Instance = this;
    }

    public void Update()
    {
        HandleDebugInput();
        SyncFpsChartState();
    }

    private void HandleDebugInput()
    {
        var keyboard = Keyboard.GetState();

        // F1 - Toggle Debug Info
        if (keyboard.IsKeyDown(Keys.F1) && !_previousF1State)
        {
            ShowDebugInfo = !ShowDebugInfo;
        }
        _previousF1State = keyboard.IsKeyDown(Keys.F1);

        // F2 - Toggle Collision Boxes
        if (keyboard.IsKeyDown(Keys.F2) && !_previousF2State)
        {
            ShowCollisionBoxes = !ShowCollisionBoxes;
        }
        _previousF2State = keyboard.IsKeyDown(Keys.F2);

        // F3 - Toggle FPS Display
        if (keyboard.IsKeyDown(Keys.F3) && !_previousF3State)
        {
            ShowFps = !ShowFps;
        }
        _previousF3State = keyboard.IsKeyDown(Keys.F3);

        // F4 - Toggle Performance Stats
        if (keyboard.IsKeyDown(Keys.F4) && !_previousF4State)
        {
            ShowPerformanceStats = !ShowPerformanceStats;
        }
        _previousF4State = keyboard.IsKeyDown(Keys.F4);

        // F5 - Toggle FPS Chart
        if (keyboard.IsKeyDown(Keys.F5) && !_previousF5State)
        {
            ShowFpsChart = !ShowFpsChart;
            // Also toggle Myra window visibility
            if (GameRoot.Myra?.FpsWindow != null)
            {
                GameRoot.Myra.FpsWindow.IsVisible = ShowFpsChart;
            }
        }
        _previousF5State = keyboard.IsKeyDown(Keys.F5);

        // F11 - Toggle Fullscreen
        if (keyboard.IsKeyDown(Keys.F11) && !_previousF11State)
        {
            ToggleFullscreen();
        }
        _previousF11State = keyboard.IsKeyDown(Keys.F11);
    }

    private void ToggleFullscreen()
    {
        IsFullscreen = !IsFullscreen;
        FullscreenChanged?.Invoke(IsFullscreen);
    }

    private void SyncFpsChartState()
    {
        // Sync GameManager state with actual window visibility
        if (GameRoot.Myra?.FpsWindow != null)
        {
            ShowFpsChart = GameRoot.Myra.FpsWindow.IsVisible;
        }
    }

    // Debug helper methods
    public string GetDebugInfo()
    {
        return $"Resolution: {WindowWidth}x{WindowHeight}\n" +
               $"FPS Target: {TargetFps}\n" +
               $"Fullscreen: {IsFullscreen}\n" +
               $"Collision Boxes: {ShowCollisionBoxes}\n" +
               $"Debug Info: {ShowDebugInfo}\n" +
               $"FPS Chart: {ShowFpsChart}\n" +
               $"Player Speed: {PlayerMoveSpeed}\n" +
               $"Camera Distance: {CameraDistance}\n" +
               $"Camera Tilt: {CameraTilt}°";
    }
}