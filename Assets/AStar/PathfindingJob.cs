using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

//Doesn't yet use burst compile
public struct PathfindingJob : IJob
{
    public NativeArray<AStarNode> grid;
    public int gridWidth;
    public int gridHeight;
    public int startIndex;
    public int targetIndex;

    public NativeList<Vector3> pathPositions; //The final output of the job, it's the positions of each point in the path

    public void Execute()
    {
        FindPath(startIndex, targetIndex);

        
    }

    ///<summary> yeet </summary>
    void FindPath(int _startIndex, int _targetIndex)
    {

        BinaryHeap<NodeIndexCost> openNodes = new BinaryHeap<NodeIndexCost>(gridWidth * gridHeight, Allocator.Temp);
        NativeList<int> closedNodes = new NativeList<int>(0, Allocator.Temp);

        if (grid[_targetIndex].walkable == false) { _targetIndex = GetCloestWalkable(_targetIndex); }

        openNodes.PushElement(new NodeIndexCost(_startIndex, grid[_startIndex].fCost));

        AStarNode targetNode = grid[_targetIndex];
        
        while (openNodes.currentHeapCount > 0)
        {
            AStarNode currentNode = grid[openNodes.PeekElement().index];
            

            openNodes.PopElement();
            closedNodes.Add(currentNode.gridIndex);

            if (currentNode.gridIndex == targetNode.gridIndex)
            {
                //Path found
                openNodes.Dispose();
                closedNodes.Dispose();
                GetNodePositions(RetracePath(grid[_startIndex], grid[_targetIndex]));
                return;
            }

            NativeList<AStarNode> neighbours = GetNeighbours(currentNode);
            for (int i = 0; i < neighbours.Length; i++)
            {
                AStarNode neighbour = neighbours[i];
                if (neighbour.walkable == false || closedNodes.Contains(neighbour.gridIndex)) { continue; }

                int neighbourGCost = currentNode.gCost + GetNodeDistance(currentNode, neighbour);
                NodeIndexCost neighbourNodeIndexCost = new NodeIndexCost(neighbour.gridIndex, neighbour.fCost);
                //openNodes.heapElements.con
                
                if (neighbourGCost < neighbour.gCost || !openNodes.heapElements.Contains(neighbourNodeIndexCost))
                {
                    AStarNode updatedNeighbour = grid[neighbour.gridIndex];
                    updatedNeighbour.gCost = neighbourGCost;
                    updatedNeighbour.hCost = GetNodeDistance(neighbour, targetNode);
                    updatedNeighbour.parent = currentNode.gridIndex;
                    grid[neighbour.gridIndex] = updatedNeighbour;
                    if (!openNodes.heapElements.Contains(neighbourNodeIndexCost)) { openNodes.PushElement(new NodeIndexCost(updatedNeighbour.gridIndex,updatedNeighbour.fCost)); }
                }
            }
           

        }
        openNodes.Dispose();
        closedNodes.Dispose();
        
    }

    private int GetCloestWalkable(int _unwalkableIndex)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; } //Node that was passed in
                Vector2Int index2D = PybUtility.IndexTo2D(_unwalkableIndex, gridWidth);
                index2D.x += x;
                index2D.y += y;
                int currentIndex = PybUtility.IndexTo1D(index2D, gridWidth);
                if (grid[currentIndex].walkable)
                {
                    return currentIndex;
                }
            }
        }
        return _unwalkableIndex;
    }

    public NativeList<AStarNode> GetNeighbours(AStarNode node)
    {
        NativeList<AStarNode> neighbours = new NativeList<AStarNode>(0, Allocator.Temp);
        Vector2Int gridIndex = PybUtility.IndexTo2D(node.gridIndex, gridWidth);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; } //Node that was passed in
                Vector2Int currentNodeGridIndex = new Vector2Int(gridIndex.x + x, gridIndex.y + y);
                if (currentNodeGridIndex.x >= 0 && currentNodeGridIndex.x < gridWidth && currentNodeGridIndex.y >= 0 && currentNodeGridIndex.y < gridHeight)
                {
                    neighbours.Add(grid[PybUtility.IndexTo1D(currentNodeGridIndex, gridWidth)]);
                }
            }
        }
        return neighbours;
    }

    int GetNodeDistance(AStarNode startNode, AStarNode endNode)
    {
        Vector2Int startGridPoint = PybUtility.IndexTo2D(startNode.gridIndex, gridWidth);
        Vector2Int endGridPoint = PybUtility.IndexTo2D(endNode.gridIndex, gridWidth);
        int xDelta = Mathf.Abs(startGridPoint.x - endGridPoint.x);
        int yDelta = Mathf.Abs(startGridPoint.y - endGridPoint.y);

        if (xDelta > yDelta) { return (14 * yDelta) + ((xDelta - yDelta) * 10); }

        return (14 * xDelta) + ((yDelta - xDelta) * 10);
    }

    NativeList<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        NativeList<AStarNode> path = new NativeList<AStarNode>(0, Allocator.Temp);
        AStarNode currentNode = endNode;
        while (currentNode.gridIndex != startNode.gridIndex)
        {
            path.Add(currentNode);
            currentNode = grid[currentNode.parent];
        }
        return ReverseNativeList(path);
    }

    NativeList<T> ReverseNativeList<T>(NativeList<T> _list) where T:struct
    {
        NativeList<T> reversedList = new NativeList<T>(0, Allocator.Temp);
        for (int i = 0; i < _list.Length; i++)
        {
            reversedList.Add(_list[_list.Length - 1 - i]);
        }
        return reversedList;
    }

    void GetNodePositions(NativeList<AStarNode> _NodeList)
    {
        for (int i = 0; i < _NodeList.Length; i++)
        {
            pathPositions.Add(_NodeList[i].position);
        }
    }
}
public struct NodeIndexCost : IComparable<NodeIndexCost>, IEquatable<NodeIndexCost>
{
    //This struct contains the node index, and whatever cost you want to sort by in the BinaryHeap
    public int index;
    public int value;
    public NodeIndexCost(int _index, int _value)
    {
        this.index = _index;
        this.value = _value;
    }
    public int CompareTo(NodeIndexCost obj)
    {
        return value.CompareTo(obj.value);
    }

    public bool Equals(NodeIndexCost other)
    {
        if (index == other.index) { return true; }
        return false;
    }
}