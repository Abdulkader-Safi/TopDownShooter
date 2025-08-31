using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TopDownShooter.Game.Core;

public class GameManager
{
    public static GameManager Instance { get; private set; }

    // Graphics Settings
    public int WindowWidth { get; private set; } = 1280;
    public int WindowHeight { get; private set; } = 720;
    public string WindowTitle { get; private set; } = "Top Down Shooter - MonoGame";
    public bool IsFullscreen { get; private set; } = false;
    public bool VSync { get; private set; } = true;

    // Debug Settings
    public bool ShowCollisionBoxes { get; private set; } = true;
    public bool ShowDebugInfo { get; private set; } = false;
    public bool ShowFps { get; private set; } = true;
    public bool ShowPlayerOutline { get; set; } = true;

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

        // F11 - Toggle Fullscreen
        if (keyboard.IsKeyDown(Keys.F11) && !_previousF11State)
        {
            ToggleFullscreen();
        }
        _previousF11State = keyboard.IsKeyDown(Keys.F11);
    }

    private void SetResolution(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        WindowWidth = width;
        WindowHeight = height;
        ResolutionChanged?.Invoke(width, height);
    }

    private void ToggleFullscreen()
    {
        IsFullscreen = !IsFullscreen;
        FullscreenChanged?.Invoke(IsFullscreen);
    }

    public void SetWindowTitle(string title)
    {
        if (string.IsNullOrEmpty(title)) return;
        WindowTitle = title;
        WindowTitleChanged?.Invoke(title);
    }

    private void SetTargetFps(int fps)
    {
        if (fps is > 0 and <= 300)
        {
            TargetFps = fps;
        }
    }

    // Preset resolution methods
    private void SetResolution720P() => SetResolution(1280, 720);
    private void SetResolution1080P() => SetResolution(1920, 1080);
    public void SetResolution1440P() => SetResolution(2560, 1440);
    public void SetResolutionCustom(int width, int height) => SetResolution(width, height);

    // Performance preset methods
    public void SetLowQualityPreset()
    {
        SetResolution720P();
        SetTargetFps(30);
        ShowCollisionBoxes = false;
        ShowDebugInfo = false;
        ShowPerformanceStats = false;
    }

    public void SetMediumQualityPreset()
    {
        SetResolution720P();
        SetTargetFps(60);
        ShowCollisionBoxes = true;
        ShowDebugInfo = false;
        ShowPerformanceStats = false;
    }

    public void SetHighQualityPreset()
    {
        SetResolution1080P();
        SetTargetFps(60);
        ShowCollisionBoxes = true;
        ShowDebugInfo = true;
        ShowPerformanceStats = true;
    }

    // Debug helper methods
    public string GetDebugInfo()
    {
        return $"Resolution: {WindowWidth}x{WindowHeight}\n" +
               $"FPS Target: {TargetFps}\n" +
               $"Fullscreen: {IsFullscreen}\n" +
               $"Collision Boxes: {ShowCollisionBoxes}\n" +
               $"Debug Info: {ShowDebugInfo}\n" +
               $"Player Speed: {PlayerMoveSpeed}\n" +
               $"Camera Distance: {CameraDistance}\n" +
               $"Camera Tilt: {CameraTilt}°";
    }

    public void ResetToDefaults()
    {
        WindowWidth = 1280;
        WindowHeight = 720;
        WindowTitle = "Top Down Shooter - MonoGame";
        IsFullscreen = false;
        VSync = true;

        ShowCollisionBoxes = true;
        ShowDebugInfo = false;
        ShowFps = true;
        ShowPlayerOutline = true;

        PlayerMoveSpeed = 5f;
        PlayerDashForce = 15f;
        PlayerDashCooldown = 1f;
        CameraDistance = 18f;
        CameraTilt = 55f;

        MasterVolume = 1f;
        SfxVolume = 0.8f;
        MusicVolume = 0.6f;

        TargetFps = 60;
        ShowPerformanceStats = false;

        // Trigger events to update the game
        ResolutionChanged?.Invoke(WindowWidth, WindowHeight);
        FullscreenChanged?.Invoke(IsFullscreen);
        WindowTitleChanged?.Invoke(WindowTitle);
    }
}