using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public LayerMask unwalkableLayer;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    float nodeDiameter => nodeRadius * 2;
    AStarNode[,] grid;

    Vector2Int gridNodeSize;

    private void Start()
    {
        GetGridNodeSize();
        CreateGrid();
    }

    private void GetGridNodeSize()
    {
        
        gridNodeSize.x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridNodeSize.y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }
    private void CreateGrid()
    {
        grid = new AStarNode[gridNodeSize.x, gridNodeSize.y];

        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2);
        for (int x = 0; x < gridNodeSize.x; x++)
        {
            for (int y = 0; y < gridNodeSize.y; y++)
            {
                Vector3 nodePosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(nodePosition, nodeRadius, unwalkableLayer);
                grid[x, y] = new AStarNode(walkable, nodePosition);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            foreach (AStarNode n in grid)
            {
                
                Gizmos.color = n.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter / 2f));
            }
        }
    }

    public Vector2Int WorldToGridPoint(Vector3 worldPos, bool clamp = true)
    {
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2);
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = (int)((worldPos.x - worldBottomLeft.x) / nodeDiameter);
        gridPos.y = (int)((worldPos.z - worldBottomLeft.z) / nodeDiameter);

        if (clamp)
        {
            gridPos.x = Mathf.Clamp(gridPos.x, 0, gridNodeSize.x - 1);
            gridPos.y = Mathf.Clamp(gridPos.y, 0, gridNodeSize.y - 1);
        }
      
        return gridPos;
    }

    public AStarNode GetNodeAtPoint(Vector3 p)
    {
        Vector2Int nodePoint = WorldToGridPoint(p);
        return grid[nodePoint.x, nodePoint.y];
    }
    public AStarNode GetNodeAtPoint(Vector2Int p)
    {
        return grid[p.x, p.y];
    }


    public List<AStarNode> GetNeighbours(AStarNode node)
    {
        List<AStarNode> neighbours = new List<AStarNode>();
        Vector2Int nodeGridPoint = WorldToGridPoint(node.position);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; } //Node that was passed in
                Vector2Int currentNodeGridPos = new Vector2Int(nodeGridPoint.x + x, nodeGridPoint.y + y);
                if (currentNodeGridPos.x >= 0 && currentNodeGridPos.x < gridNodeSize.x && currentNodeGridPos.y >= 0 && currentNodeGridPos.y < gridNodeSize.y)
                {
                    neighbours.Add(GetNodeAtPoint(currentNodeGridPos));
                }
            }
        }
        return neighbours;
    }


}
