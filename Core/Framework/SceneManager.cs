namespace Core.Framework;

public class SceneManager
{
    private Scene _currentScene;
    private Scene _nextScene;
    
    public Scene CurrentScene => _currentScene;
    
    public void LoadScene(Scene scene)
    {
        _nextScene = scene;
    }
    
    public void Update()
    {
        if (_nextScene != null)
        {
            _currentScene = _nextScene;
            _nextScene = null;
            
            _currentScene.Initialize();
            _currentScene.LoadContent();
        }
        
        _currentScene?.Update();
    }
    
    public void Draw()
    {
        _currentScene?.Draw();
    }
}