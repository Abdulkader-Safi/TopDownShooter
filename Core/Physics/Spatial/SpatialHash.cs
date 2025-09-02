using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core.Physics.Spatial;

public class SpatialHash(float cellSize)
{
    private readonly Dictionary<Vector2, List<ICollider>> _grid = new Dictionary<Vector2, List<ICollider>>();

    public void Insert(ICollider collider)
    {
        var bounds = collider.GetBounds();
        var minCell = GetCellCoord(bounds.Min.X, bounds.Min.Z);
        var maxCell = GetCellCoord(bounds.Max.X, bounds.Max.Z);

        for (var x = (int)minCell.X; x <= maxCell.X; x++)
        {
            for (var z = (int)minCell.Y; z <= maxCell.Y; z++)
            {
                var cellKey = new Vector2(x, z);
                if (!_grid.ContainsKey(cellKey))
                    _grid[cellKey] = new List<ICollider>();

                _grid[cellKey].Add(collider);
            }
        }
    }

    public IEnumerable<ICollider> QueryRadius(Vector3 position, float radius)
    {
        var center = GetCellCoord(position.X, position.Z);
        var cellRadius = (int)Math.Ceiling(radius / cellSize);
        var results = new HashSet<ICollider>();

        for (int x = (int)center.X - cellRadius; x <= center.X + cellRadius; x++)
        {
            for (int z = (int)center.Y - cellRadius; z <= center.Y + cellRadius; z++)
            {
                var cellKey = new Vector2(x, z);
                if (_grid.TryGetValue(cellKey, out var colliders))
                {
                    foreach (var collider in colliders)
                        results.Add(collider);
                }
            }
        }

        return results;
    }

    private Vector2 GetCellCoord(float x, float z)
    {
        return new Vector2(
            (float)Math.Floor(x / cellSize),
            (float)Math.Floor(z / cellSize)
        );
    }

    public void Clear()
    {
        _grid.Clear();
    }
}