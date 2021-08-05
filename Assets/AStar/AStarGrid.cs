using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    [SerializeField] private LayerMask unwalkableLayer;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private float nodeRadius;
    private float nodeDiameter => nodeRadius * 2;
    [HideInInspector] public Vector2Int gridNodeSize;
    private AStarNode[,] grid;
    [HideInInspector] public AStarNode[] flatGrid;

    private void Start()
    {
        CalculateGridNodeSize();
        CreateGrid();
    }


    private void CalculateGridNodeSize()
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
                grid[x, y] = new AStarNode(walkable, nodePosition, IndexTo1D(new Vector2Int(x, y), gridNodeSize.x));
            }
        }
        flatGrid = GridTo1D();
    }

    private AStarNode[] GridTo1D()
    {
        AStarNode[] newGrid = new AStarNode[gridNodeSize.x * gridNodeSize.y];
        for (int x = 0; x < gridNodeSize.x; x++)
        {
            for (int y = 0; y < gridNodeSize.y; y++)
            {
                newGrid[IndexTo1D(new Vector2Int(x, y), gridNodeSize.x)] = grid[x, y];
            }
        }
        return newGrid;
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


    //Note to self - Create Extension method to house methods like this.
    private int IndexTo1D(Vector2Int _2DIndex, int _arrayWidth)
    {
        return _2DIndex.x + (_2DIndex.y * _arrayWidth);
    }



    //Debugging-----------------------------------------------------------------------------------------------------------
    /*
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
    */

}
