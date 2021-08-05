using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct PathfindingJob : IJob
{
    public NativeArray<AStarNode> grid;
    public int gridWidth;
    public int gridHeight;
    public int startIndex;
    public int targetIndex;

    public NativeArray<Vector3> pathPositions; //The final output of the job, it's the positions of each point in the path

    public void Execute()
    {
        pathPositions = FindPath(startIndex, targetIndex);
    }

    ///<summary> yeet </summary>
    NativeArray<Vector3> FindPath(int startIndex, int targetIndex)
    {

        NativeList<int> openNodes = new NativeList<int>(0, Allocator.Temp);  //This references the grid 1D index of a node in grid
        NativeList<int> closedNodes = new NativeList<int>(0, Allocator.Temp);

        openNodes.Add(startIndex);

        AStarNode targetNode = grid[targetIndex];
        AStarNode startNode = grid[startIndex];
        
        while (openNodes.Length > 0)
        {
            AStarNode currentNode = grid[openNodes[0]];
            //Find lowest f-cost node in openNodes [VERY SLOW]
            int currentNodeOpenIndex = 0;   //The index of the currentNode inside of openNodes
            for (int i = 1; i < openNodes.Length; i++)
            {
                AStarNode currentOpenNode = grid[openNodes[i]];
                if (currentOpenNode.fCost < currentNode.fCost || (currentOpenNode.fCost == currentNode.fCost && currentOpenNode.hCost < currentNode.hCost))
                {
                    currentNode = currentOpenNode;
                    currentNodeOpenIndex = i;
                }
            }

            openNodes.RemoveAt(currentNodeOpenIndex);
            closedNodes.Add(currentNode.gridIndex);

            if (currentNode.gridIndex == targetNode.gridIndex)
            {
                //Path found
                openNodes.Dispose();
                closedNodes.Dispose();
                return GetNodePositions(RetracePath(startNode, targetNode));
            }

            foreach (AStarNode neighbour in GetNeighbours(currentNode))
            {
                if (neighbour.walkable == false || closedNodes.Contains(neighbour.gridIndex)) { continue; }

                int neighbourGCost = currentNode.gCost + GetNodeDistance(currentNode, neighbour);
                if (neighbourGCost < neighbour.gCost || !openNodes.Contains(neighbour.gridIndex))
                {
                    AStarNode updatedNeighbour = grid[neighbour.gridIndex];
                    updatedNeighbour.gCost = neighbourGCost;
                    updatedNeighbour.hCost = GetNodeDistance(neighbour, targetNode);
                    updatedNeighbour.parent = currentNode.gridIndex;
                    grid[neighbour.gridIndex] = updatedNeighbour;
                    if (!openNodes.Contains(updatedNeighbour.gridIndex)) { openNodes.Add(updatedNeighbour.gridIndex); }
                }
            }

        }
        openNodes.Dispose();
        closedNodes.Dispose();
        return pathPositions;  //This should never be called
        
    }


    public NativeList<AStarNode> GetNeighbours(AStarNode node)
    {
        NativeList<AStarNode> neighbours = new NativeList<AStarNode>();
        Vector2Int gridIndex = IndexTo2D(node.gridIndex, gridWidth);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; } //Node that was passed in
                Vector2Int currentNodeGridIndex = new Vector2Int(gridIndex.x + x, gridIndex.y + y);
                if (currentNodeGridIndex.x >= 0 && currentNodeGridIndex.x < gridWidth && currentNodeGridIndex.y >= 0 && currentNodeGridIndex.y < gridHeight)
                {
                    neighbours.Add(grid[IndexTo1D(currentNodeGridIndex, gridWidth)]);
                }
            }
        }
        return neighbours;
    }



    public Vector2Int IndexTo2D(int _1DIndex, int _arrayWidth)
    {
        int x = _1DIndex % _arrayWidth;
        int y = _1DIndex / _arrayWidth;
        return new Vector2Int(x, y);
    }
    public int IndexTo1D(Vector2Int _2DIndex, int _arrayWidth)
    {
        return _2DIndex.x + (_2DIndex.y * _arrayWidth);
    }

    int GetNodeDistance(AStarNode startNode, AStarNode endNode)
    {
        Vector2Int startGridPoint = IndexTo2D(startNode.gridIndex, gridWidth);
        Vector2Int endGridPoint = IndexTo2D(endNode.gridIndex, gridWidth);
        int xDelta = Mathf.Abs(startGridPoint.x - endGridPoint.x);
        int yDelta = Mathf.Abs(startGridPoint.y - endGridPoint.y);

        if (xDelta > yDelta) { return (14 * yDelta) + ((xDelta - yDelta) * 10); }

        return (14 * xDelta) + ((yDelta - xDelta) * 10);
    }

    NativeList<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        NativeList<AStarNode> path = new NativeList<AStarNode>();
        AStarNode currentNode = endNode;
        path.Add(currentNode);
        while (currentNode.gridIndex != startNode.gridIndex)
        {
            currentNode = grid[currentNode.parent];
            path.Add(currentNode);
        }
        return ReverseNativeList(path);
    }

    NativeList<T> ReverseNativeList<T>(NativeList<T> _list) where T:struct
    {
        NativeList<T> reversedList = new NativeList<T>();
        for (int i = 0; i < _list.Length; i++)
        {
            reversedList.Add(_list[_list.Length - 1 - i]);
        }
        return reversedList;
    }

    NativeArray<Vector3> GetNodePositions(NativeList<AStarNode> _NodeList)
    {
        NativeArray<Vector3> nodePositions = new NativeArray<Vector3>(_NodeList.Length, Allocator.Temp);
        for (int i = 0; i < _NodeList.Length; i++)
        {
            nodePositions[i] = _NodeList[i].position;
        }
        return nodePositions;
    }

}
