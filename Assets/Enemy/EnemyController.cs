using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    AStarGrid gridController;
    EnemyMovement enemyMovement;
    private Transform playerTransform;

    private bool isCalculatingPath = false; //Makes sure not to start another job until the current one is finished.
    [SerializeField] private float pathUpdateFrequency = 1f;
    private float lastPathUpdate = 0f;

    private void Awake()
    {
        gridController = GameObject.FindGameObjectWithTag("ScriptObject").GetComponent<AStarGrid>();
        enemyMovement = GetComponent<EnemyMovement>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        lastPathUpdate += Time.deltaTime;
        if (lastPathUpdate >= pathUpdateFrequency && isCalculatingPath == false)
        {
            lastPathUpdate = 0f;
            isCalculatingPath = true;
            GetNewPath();
        }
    }

    private void GetNewPath()
    {
        NativeList<Vector3> result = new NativeList<Vector3>(Allocator.TempJob);

        PathfindingJob jobData = new PathfindingJob();
        jobData.gridWidth = gridController.gridNodeSize.x;
        jobData.gridHeight = gridController.gridNodeSize.y;
        jobData.startIndex = IndexTo1D(gridController.WorldToGridPoint(transform.position), gridController.gridNodeSize.x);
        jobData.targetIndex = IndexTo1D(gridController.WorldToGridPoint(playerTransform.position), gridController.gridNodeSize.x);
        NativeArray<AStarNode> grid = new NativeArray<AStarNode>(gridController.flatGrid.Length, Allocator.TempJob);
        jobData.grid = grid;
        jobData.grid.CopyFrom(gridController.flatGrid);
        jobData.pathPositions = result;

        JobHandle handle = jobData.Schedule();
        handle.Complete();

        Vector3[] newPath = new Vector3[result.Length];
        for (int i = 0; i < result.Length; i++)
        {
            newPath[i] = result[i];
        }
        enemyMovement.SetPath(newPath);

        result.Dispose();
        grid.Dispose();
        isCalculatingPath = false;

    }





    //Note to self - Create Extension method to house methods like this.
    private int IndexTo1D(Vector2Int _2DIndex, int _arrayWidth)
    {
        return _2DIndex.x + (_2DIndex.y * _arrayWidth);
    }

}
