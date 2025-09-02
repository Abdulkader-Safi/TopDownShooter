using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Services;

public class AssetService
{
    private ContentManager _content;
    private GraphicsDevice _graphicsDevice;
    
    public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
    {
        _content = content;
        _graphicsDevice = graphicsDevice;
    }
    
    public T LoadContent<T>(string assetName)
    {
        return _content.Load<T>(assetName);
    }
}