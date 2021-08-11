using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PybUtility
{
    /// <summary>
    /// Converts a 1D array index into a 2D array index
    /// </summary>
    public static Vector2Int IndexTo2D(int _1DIndex, int _arrayWidth)
    {
        int x = _1DIndex % _arrayWidth;
        int y = _1DIndex / _arrayWidth;
        return new Vector2Int(x, y);
    }
    /// <summary>
    /// Converts a 2D array index into a 1D array index
    /// </summary>
    public static int IndexTo1D(Vector2Int _2DIndex, int _arrayWidth)
    {
        return _2DIndex.x + (_2DIndex.y * _arrayWidth);
    }
}
