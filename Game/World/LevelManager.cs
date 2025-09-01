using System;
using TopDownShooter.Game.Core;
using TopDownShooter.Game.Framework;

namespace TopDownShooter.Game.World;

public class LevelManager
{
    private static LevelManager _instance;
    public static LevelManager Instance => _instance ??= new LevelManager();

    private int _currentLevelIndex;
    private readonly Type[] _levelScenes;

    private LevelManager()
    {
        _currentLevelIndex = 0;
        _levelScenes = new[]
        {
            typeof(GameScene),
            typeof(Level2Scene)
        };
    }

    public int CurrentLevelIndex => _currentLevelIndex;
    public int TotalLevels => _levelScenes.Length;
    public bool HasNextLevel => _currentLevelIndex < _levelScenes.Length - 1;
    public bool HasPreviousLevel => _currentLevelIndex > 0;

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= _levelScenes.Length)
            return;

        _currentLevelIndex = levelIndex;
        var sceneType = _levelScenes[levelIndex];
        var scene = (Scene)Activator.CreateInstance(sceneType);
        GameRoot.Instance.SceneManager.LoadScene(scene);
    }

    public void LoadNextLevel()
    {
        if (HasNextLevel)
        {
            LoadLevel(_currentLevelIndex + 1);
        }
        else
        {
            // If at the last level, cycle back to first level
            LoadLevel(0);
        }
    }

    public void LoadPreviousLevel()
    {
        if (HasPreviousLevel)
        {
            LoadLevel(_currentLevelIndex - 1);
        }
        else
        {
            // If at the first level, go to last level
            LoadLevel(_levelScenes.Length - 1);
        }
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(_currentLevelIndex);
    }

    public void LoadFirstLevel()
    {
        LoadLevel(0);
    }
}