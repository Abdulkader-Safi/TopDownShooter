using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core.Services;

public enum MouseButton
{
    Left,
    Right,
    Middle
}

public class InputService
{
    private KeyboardState _currentKeyboard;
    private KeyboardState _previousKeyboard;
    private MouseState _currentMouse;
    private MouseState _previousMouse;
    
    public Vector2 MousePosition => new Vector2(_currentMouse.X, _currentMouse.Y);
    
    public void Update()
    {
        _previousKeyboard = _currentKeyboard;
        _previousMouse = _currentMouse;
        
        _currentKeyboard = Keyboard.GetState();
        _currentMouse = Mouse.GetState();
    }
    
    public bool IsKeyDown(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key);
    }
    
    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);
    }
    
    public bool IsKeyReleased(Keys key)
    {
        return !_currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyDown(key);
    }
    
    public bool IsMouseButtonDown(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => _currentMouse.LeftButton == ButtonState.Pressed,
            MouseButton.Right => _currentMouse.RightButton == ButtonState.Pressed,
            MouseButton.Middle => _currentMouse.MiddleButton == ButtonState.Pressed,
            _ => false
        };
    }
    
    public bool IsMouseButtonPressed(MouseButton button)
    {
        var current = button switch
        {
            MouseButton.Left => _currentMouse.LeftButton == ButtonState.Pressed,
            MouseButton.Right => _currentMouse.RightButton == ButtonState.Pressed,
            MouseButton.Middle => _currentMouse.MiddleButton == ButtonState.Pressed,
            _ => false
        };
        
        var previous = button switch
        {
            MouseButton.Left => _previousMouse.LeftButton == ButtonState.Pressed,
            MouseButton.Right => _previousMouse.RightButton == ButtonState.Pressed,
            MouseButton.Middle => _previousMouse.MiddleButton == ButtonState.Pressed,
            _ => false
        };
        
        return current && !previous;
    }
}