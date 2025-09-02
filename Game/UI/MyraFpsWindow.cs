using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using TopDownShooter.Game.Framework;

namespace TopDownShooter.Game.UI;

public class FpsChartPanel : Panel
{
    private readonly PerformanceTracker _performanceTracker;

    public FpsChartPanel(PerformanceTracker performanceTracker)
    {
        _performanceTracker = performanceTracker;
        Width = 280;
        Height = 100;
    }

    public override void InternalRender(RenderContext context)
    {
        base.InternalRender(context);

        var fpsData = _performanceTracker.FpsHistory.ToArray();
        var bounds = ActualBounds;
        var chartArea = new Rectangle(bounds.X + 5, bounds.Y + 5, bounds.Width - 10, bounds.Height - 10);

        // Draw background
        context.FillRectangle(chartArea, Color.Black * 0.3f);
        context.DrawRectangle(chartArea, Color.White);

        if (fpsData.Length < 2)
        {
            // Show "No Data" when there's insufficient data
            return;
        }

        // Calculate scale with better range handling
        var minFpsRaw = fpsData.Min();
        var maxFpsRaw = fpsData.Max();
        var fpsVariation = maxFpsRaw - minFpsRaw;

        float minFps, maxFps;

        // If FPS is very stable (less than 2 FPS variation), zoom in for better visibility
        if (fpsVariation < 2.0f)
        {
            var avgFps = fpsData.Average();
            minFps = Math.Max(0, avgFps - 3);
            maxFps = avgFps + 3;
        }
        else if (fpsVariation < 10.0f)
        {
            // Moderate variation - use tight bounds
            minFps = Math.Max(0, minFpsRaw - 2);
            maxFps = maxFpsRaw + 2;
        }
        else
        {
            // High variation - use wider bounds
            minFps = Math.Max(0, minFpsRaw - 5);
            maxFps = maxFpsRaw + 5;
        }

        var fpsRange = maxFps - minFps;
        if (fpsRange <= 0) return;

        // Draw grid lines with labels
        var gridColor = Color.Gray * 0.5f;
        for (int i = 0; i <= 4; i++)
        {
            var y = chartArea.Y + (chartArea.Height * i / 4);
            var fps = maxFps - (fpsRange * i / 4);
            context.DrawLine(new Vector2(chartArea.X, y), new Vector2(chartArea.Right, y), gridColor);
        }

        // Draw FPS line with better visibility
        if (fpsData.Length > 1)
        {
            var stepX = (float)chartArea.Width / Math.Max(1, fpsData.Length - 1);

            for (int i = 1; i < fpsData.Length; i++)
            {
                var x1 = chartArea.X + stepX * (i - 1);
                var x2 = chartArea.X + stepX * i;

                var normalizedY1 = (fpsData[i - 1] - minFps) / fpsRange;
                var normalizedY2 = (fpsData[i] - minFps) / fpsRange;

                // Clamp to avoid drawing outside bounds
                normalizedY1 = Math.Max(0, Math.Min(1, normalizedY1));
                normalizedY2 = Math.Max(0, Math.Min(1, normalizedY2));

                var y1 = chartArea.Bottom - (normalizedY1 * chartArea.Height);
                var y2 = chartArea.Bottom - (normalizedY2 * chartArea.Height);

                var color = fpsData[i] >= 54 ? Color.Green : fpsData[i] >= 42 ? Color.Yellow : Color.Red;
                context.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, 2);

                // Draw points for better visibility
                context.FillRectangle(new Rectangle((int)x2 - 1, (int)y2 - 1, 3, 3), color);
            }
        }

        // Draw target FPS line (60 FPS) - always visible
        var targetY = chartArea.Bottom - ((60 - minFps) / fpsRange) * chartArea.Height;
        if (targetY >= chartArea.Y && targetY <= chartArea.Bottom)
        {
            context.DrawLine(new Vector2(chartArea.X, targetY), new Vector2(chartArea.Right, targetY), Color.Cyan);
        }
    }
}

public class MyraFpsWindow
{
    private readonly PerformanceTracker _performanceTracker;
    private Desktop _desktop;
    private Window _window;
    private Label _fpsLabel;
    private Label _avgFpsLabel;
    private Label _minFpsLabel;
    private Label _maxFpsLabel;
    private FpsChartPanel _fpsChart;
    private bool _isVisible = true;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            if (_window != null)
            {
                if (value && !_window.Visible)
                {
                    // If we're showing the window and it was closed, re-add it to desktop
                    if (!_desktop.Widgets.Contains(_window))
                    {
                        _desktop.Widgets.Add(_window);
                    }
                }
                _window.Visible = value;
            }
        }
    }

    public MyraFpsWindow(PerformanceTracker performanceTracker)
    {
        _performanceTracker = performanceTracker;
    }

    public void Initialize(Microsoft.Xna.Framework.Game game)
    {
        MyraEnvironment.Game = game;

        _desktop = new Desktop();

        // Create main window positioned on the right side
        var viewport = game.GraphicsDevice.Viewport;
        _window = new Window
        {
            Title = "FPS Monitor",
            Width = 300,
            Height = 300,
            Left = viewport.Width - 320, // Position on the right side with 20px margin
            Top = 20, // 20px from top
            Visible = _isVisible
        };

        // Handle window close event to maintain F5 toggle functionality
        _window.Closed += OnWindowClosed;

        // Create content panel
        var panel = new VerticalStackPanel
        {
            Spacing = 8
        };

        // FPS Labels
        _fpsLabel = new Label { Text = "Current FPS: --" };
        _avgFpsLabel = new Label { Text = "Average FPS: --" };
        _minFpsLabel = new Label { Text = "Min FPS: --" };
        _maxFpsLabel = new Label { Text = "Max FPS: --" };

        // FPS Chart Panel
        _fpsChart = new FpsChartPanel(_performanceTracker);

        // Add controls to panel
        panel.Widgets.Add(_fpsLabel);
        panel.Widgets.Add(_fpsChart);
        panel.Widgets.Add(_avgFpsLabel);
        panel.Widgets.Add(_minFpsLabel);
        panel.Widgets.Add(_maxFpsLabel);

        // Add instruction and status
        panel.Widgets.Add(new Label
        {
            Text = "Press F5 to toggle",
            TextColor = Color.Gray
        });

        _window.Content = panel;
        _desktop.Widgets.Add(_window);
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        // When X is clicked, update our internal state but keep the window available for F5 toggle
        _isVisible = false;
    }

    public void Update()
    {
        if (!_isVisible) return;

        var currentFps = _performanceTracker.CurrentFps;
        var avgFps = _performanceTracker.AverageFps;
        var minFps = _performanceTracker.MinFps;
        var maxFps = _performanceTracker.MaxFps;

        // Update labels
        _fpsLabel.Text = $"Current FPS: {currentFps:F1}";
        _fpsLabel.TextColor = GetFpsColor(currentFps);

        _avgFpsLabel.Text = $"Average FPS: {avgFps:F1}";
        _minFpsLabel.Text = $"Min FPS: {minFps:F1}";
        _maxFpsLabel.Text = $"Max FPS: {maxFps:F1}";

        // Chart updates automatically in its render method
    }

    public void Draw()
    {
        if (_isVisible)
        {
            _desktop.Render();
        }
    }

    private Color GetFpsColor(float fps)
    {
        return fps >= 54 ? Color.Green :     // 90% of 60 FPS
               fps >= 42 ? Color.Yellow :    // 70% of 60 FPS
               Color.Red;
    }

    public void Dispose()
    {
        _desktop?.Dispose();
    }
}