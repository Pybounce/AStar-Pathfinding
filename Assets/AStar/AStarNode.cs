using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public bool walkable;
    public Vector3 position;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public AStarNode parent;
    public AStarNode(bool _walkable, Vector3 _position)
    {
        walkable = _walkable;
        position = _position;
    }

}
