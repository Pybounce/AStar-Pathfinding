using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    /*
    AStarGrid grid;

    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("ScriptsObject").GetComponent<AStarGrid>();
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        AStarNode startNode = grid.GetNodeAtPoint(startPos);
        AStarNode targetNode = grid.GetNodeAtPoint(targetPos);

        List<AStarNode> openNodes = new List<AStarNode>();
        List<AStarNode> closedNodes = new List<AStarNode>();

        openNodes.Add(startNode);

        while (openNodes.Count > 0)
        {
            AStarNode currentNode = openNodes[0];

            //Find lowest f-cost node in openNodes [VERY SLOW]
            for (int i = 1; i < openNodes.Count; i++)
            {
                if (openNodes[i].fCost < currentNode.fCost || (openNodes[i].fCost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost))
                {
                    currentNode = openNodes[i];
                }
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode.Equals(targetNode))
            {
                //Path found
                RetracePath(startNode, targetNode);
                return;
            }
            
            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode))
            {
                if (neighbour.walkable == false || closedNodes.Contains(neighbour)) { continue; }

                int neighbourGCost = currentNode.gCost + GetNodeDistance(currentNode, neighbour);
                if (neighbourGCost < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = neighbourGCost;
                    neighbour.hCost = GetNodeDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openNodes.Contains(neighbour)) { openNodes.Add(neighbour); }
                }
            }

        }
    }

    int GetNodeDistance(AStarNode startNode, AStarNode endNode)
    {
        Vector2Int startGridPoint = grid.WorldToGridPoint(startNode.position);
        Vector2Int endGridPoint = grid.WorldToGridPoint(endNode.position);
        int xDelta = Mathf.Abs(startGridPoint.x - endGridPoint.x);
        int yDelta = Mathf.Abs(startGridPoint.y - endGridPoint.y);

        if (xDelta > yDelta) { return (14 * yDelta) + ((xDelta - yDelta) * 10); }

        return (14 * xDelta) + ((yDelta - xDelta) * 10);
    }


    void RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;
        path.Add(currentNode);
        while (currentNode != startNode)
        {
            currentNode = currentNode.parent;
            path.Add(currentNode);
        }
        path.Reverse();
    }
    */
}
