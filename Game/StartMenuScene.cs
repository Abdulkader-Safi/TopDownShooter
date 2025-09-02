using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Framework;
using TopDownShooter.Game.Services;
using TopDownShooter.Game.World;

namespace TopDownShooter.Game;

public class StartMenuScene : Scene
{
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _pixelTexture;
    private bool _startButtonPressed = false;

    // Button properties
    private Rectangle _startButtonBounds;
    private bool _startButtonHovered = false;

    public override void Initialize()
    {
        base.Initialize();

        _spriteBatch = new SpriteBatch(GameRoot.Instance.GraphicsDevice);

        // Create a simple pixel texture for UI rendering
        _pixelTexture = new Texture2D(GameRoot.Instance.GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        // Setup start button bounds (centered on screen)
        var viewport = GameRoot.Instance.GraphicsDevice.Viewport;
        var buttonWidth = 200;
        var buttonHeight = 60;
        _startButtonBounds = new Rectangle(
            (viewport.Width - buttonWidth) / 2,
            (viewport.Height - buttonHeight) / 2,
            buttonWidth,
            buttonHeight
        );
    }

    public override void LoadContent()
    {
        base.LoadContent();
        
        // Create default system font
        _font = GameRoot.Instance.Content.Load<SpriteFont>("DefaultFont");
    }

    public override void Update()
    {
        base.Update();

        // Handle input
        var mouseState = Mouse.GetState();
        var mousePosition = mouseState.Position;

        // Check if mouse is over start button
        _startButtonHovered = _startButtonBounds.Contains(mousePosition);

        // Check for start button click
        if (_startButtonHovered && GameRoot.Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _startButtonPressed = true;
        }

        // Check for Enter key press as alternative
        if (GameRoot.Input.IsKeyPressed(Keys.Enter) || GameRoot.Input.IsKeyPressed(Keys.Space))
        {
            _startButtonPressed = true;
        }

        // Start the game if button was pressed
        if (_startButtonPressed)
        {
            LevelManager.Instance.LoadFirstLevel();
        }
    }

    public override void Draw()
    {
        var device = GameRoot.Instance.GraphicsDevice;
        device.Clear(Color.Black);

        _spriteBatch.Begin();

        // Draw title
        var titleText = "TOP DOWN SHOOTER";
        var titleSize = _font.MeasureString(titleText);
        var titlePosition = new Vector2(
            (device.Viewport.Width - titleSize.X) / 2,
            device.Viewport.Height / 2 - 100
        );
        _spriteBatch.DrawString(_font, titleText, titlePosition, Color.White);

        // Draw start button
        var buttonColor = _startButtonHovered ? Color.Gray : Color.DarkGray;
        DrawRectangle(_spriteBatch, _startButtonBounds, buttonColor);

        // Draw button border
        DrawRectangleBorder(_spriteBatch, _startButtonBounds, Color.White, 2);

        // Draw button text
        var buttonText = "START";
        var textSize = _font.MeasureString(buttonText);
        var textPosition = new Vector2(
            _startButtonBounds.X + (_startButtonBounds.Width - textSize.X) / 2,
            _startButtonBounds.Y + (_startButtonBounds.Height - textSize.Y) / 2
        );
        _spriteBatch.DrawString(_font, buttonText, textPosition, Color.White);

        // Draw instructions
        var instructionText = "Click START or press ENTER/SPACE to play";
        var instructionSize = _font.MeasureString(instructionText);
        var instructionPosition = new Vector2(
            (device.Viewport.Width - instructionSize.X) / 2,
            _startButtonBounds.Bottom + 40
        );
        _spriteBatch.DrawString(_font, instructionText, instructionPosition, Color.Gray);

        _spriteBatch.End();
    }

    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        spriteBatch.Draw(_pixelTexture, rectangle, color);
    }

    private void DrawRectangleBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int borderWidth)
    {
        // Top
        spriteBatch.Draw(_pixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, borderWidth), color);
        // Bottom
        spriteBatch.Draw(_pixelTexture, new Rectangle(rectangle.X, rectangle.Bottom - borderWidth, rectangle.Width, borderWidth), color);
        // Left
        spriteBatch.Draw(_pixelTexture, new Rectangle(rectangle.X, rectangle.Y, borderWidth, rectangle.Height), color);
        // Right
        spriteBatch.Draw(_pixelTexture, new Rectangle(rectangle.Right - borderWidth, rectangle.Y, borderWidth, rectangle.Height), color);
    }

}