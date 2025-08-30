using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownShooter.Game.Rendering;

public class Camera
{
    public static Camera Current { get; set; }
    
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    
    private GraphicsDevice _graphicsDevice;
    
    public Camera(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        ResizeViewport(graphicsDevice);
        Current = this;
    }
    
    public void ResizeViewport(GraphicsDevice graphicsDevice)
    {
        var viewport = graphicsDevice.Viewport;
        float aspectRatio = (float)viewport.Width / viewport.Height;
        
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            aspectRatio,
            0.1f,
            1000f
        );
    }
    
    public void SetTopDownFollow(Vector3 focusXZ, float distance = 18f, float tiltDeg = 55f)
    {
        var tiltRad = MathHelper.ToRadians(tiltDeg);
        
        // Position camera behind and above the focus point
        var offset = new Vector3(0, distance * (float)Math.Sin(tiltRad), -distance * (float)Math.Cos(tiltRad));
        Position = focusXZ + offset;
        Target = focusXZ;
        
        UpdateView();
    }
    
    public void UpdateView()
    {
        View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
    }
    
    public Ray ScreenToRay(Vector2 screenPos)
    {
        var viewport = _graphicsDevice.Viewport;
        
        var nearPoint = viewport.Unproject(
            new Vector3(screenPos.X, screenPos.Y, 0f),
            Projection, View, Matrix.Identity);
            
        var farPoint = viewport.Unproject(
            new Vector3(screenPos.X, screenPos.Y, 1f),
            Projection, View, Matrix.Identity);
            
        var direction = Vector3.Normalize(farPoint - nearPoint);
        return new Ray(nearPoint, direction);
    }
}