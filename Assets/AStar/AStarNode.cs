using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AStarNode
{
    public bool walkable;
    public Vector3 position;
    public int gridIndex;
    public int gCost;
    public int hCost;
    public int weightCost;
    public int fCost => (gCost + hCost) * weightCost;
    public int parent;   //The index in the grid array (flattened grid)
    public AStarNode(bool _walkable, Vector3 _position, int _gridIndex)
    {
        walkable = _walkable;
        position = _position;
        gridIndex = _gridIndex;
        gCost = 0;
        hCost = 0;
        weightCost = 1;
        parent = 0;
    }

}
