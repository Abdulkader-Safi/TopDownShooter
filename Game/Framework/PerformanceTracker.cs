using System;
using System.Collections.Generic;
using System.Linq;

namespace TopDownShooter.Game.Framework;

public class PerformanceTracker
{
    private readonly Queue<float> _fpsHistory;
    private readonly int _maxSamples;
    private float _currentFps;
    private float _minFps = float.MaxValue;
    private float _maxFps = float.MinValue;
    private float _averageFps;
    
    public float CurrentFps => _currentFps;
    public float MinFps => _minFps == float.MaxValue ? 0f : _minFps;
    public float MaxFps => _maxFps == float.MinValue ? 0f : _maxFps;
    public float AverageFps => _averageFps;
    public IReadOnlyCollection<float> FpsHistory => _fpsHistory;
    
    public PerformanceTracker(int maxSamples = 60)
    {
        _maxSamples = maxSamples;
        _fpsHistory = new Queue<float>(maxSamples);
    }
    
    public void Update(float deltaTime)
    {
        if (deltaTime <= 0f) return;
        
        _currentFps = 1.0f / deltaTime;
        
        _fpsHistory.Enqueue(_currentFps);
        
        if (_fpsHistory.Count > _maxSamples)
        {
            _fpsHistory.Dequeue();
        }
        
        UpdateStatistics();
    }
    
    private void UpdateStatistics()
    {
        if (_fpsHistory.Count == 0) return;
        
        _minFps = _fpsHistory.Min();
        _maxFps = _fpsHistory.Max();
        _averageFps = _fpsHistory.Average();
    }
    
    public float[] GetFpsArray()
    {
        return _fpsHistory.ToArray();
    }
    
    public void Reset()
    {
        _fpsHistory.Clear();
        _minFps = float.MaxValue;
        _maxFps = float.MinValue;
        _averageFps = 0f;
        _currentFps = 0f;
    }
}