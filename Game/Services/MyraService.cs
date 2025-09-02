using Microsoft.Xna.Framework;
using TopDownShooter.Game.UI;
using TopDownShooter.Game.Framework;

namespace TopDownShooter.Game.Services;

public class MyraService
{
    private bool _isInitialized;
    private MyraFpsWindow _myraFpsWindow;

    public bool IsInitialized => _isInitialized;
    public MyraFpsWindow FpsWindow => _myraFpsWindow;

    public void Initialize(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, Microsoft.Xna.Framework.Game game, PerformanceTracker performanceTracker)
    {
        if (_isInitialized) return;
        
        _myraFpsWindow = new MyraFpsWindow(performanceTracker);
        _myraFpsWindow.Initialize(game);
        
        _isInitialized = true;
    }

    public void Update(GameTime gameTime)
    {
        if (_isInitialized)
        {
            _myraFpsWindow?.Update();
        }
    }

    public void Draw()
    {
        if (_isInitialized)
        {
            _myraFpsWindow?.Draw();
        }
    }

    public void Dispose()
    {
        _myraFpsWindow?.Dispose();
        _isInitialized = false;
    }
}