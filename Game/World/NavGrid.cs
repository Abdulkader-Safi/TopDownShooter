using System;
using Microsoft.Xna.Framework;

namespace Game.World;

public class NavGrid
{
    private readonly int _width;
    private readonly int _height;
    private readonly float _cellSize;
    private readonly bool[,] _walkable;

    public NavGrid(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _walkable = new bool[width, height];

        // Initialize all cells as walkable
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _walkable[x, y] = true;
            }
        }
    }

    public void SetWalkable(int x, int y, bool walkable)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
            _walkable[x, y] = walkable;
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
            return false;
        return _walkable[x, y];
    }

    public Vector2 WorldToGrid(Vector3 worldPos)
    {
        return new Vector2(
            (int)Math.Floor((worldPos.X + _width * _cellSize * 0.5f) / _cellSize),
            (int)Math.Floor((worldPos.Z + _height * _cellSize * 0.5f) / _cellSize)
        );
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(
            x * _cellSize - _width * _cellSize * 0.5f,
            0,
            y * _cellSize - _height * _cellSize * 0.5f
        );
    }
}