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
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private float pathUpdateFrequency = 1f;
    private float lastPathUpdate = 0f;


    private void Start()
    {
        gridController = GameObject.FindGameObjectWithTag("ScriptObject").GetComponent<AStarGrid>();
        enemyMovement = this.GetComponent<EnemyMovement>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        lastPathUpdate += Time.deltaTime;
        if (IsPlayerPathClear())
        {
            enemyMovement.followPlayer = true;
            lastPathUpdate = pathUpdateFrequency;   //Ensures that when they no longer follow the player, it updates the path immediately
        }
        else if (lastPathUpdate >= pathUpdateFrequency)
        {

            enemyMovement.followPlayer = false;
            lastPathUpdate = 0f;
            GetNewPath();

        }
    }
    

    private void GetNewPath()
    {
        NativeList<Vector3> result = new NativeList<Vector3>(Allocator.TempJob);

        PathfindingJob jobData = new PathfindingJob();
        jobData.gridWidth = gridController.gridNodeSize.x;
        jobData.gridHeight = gridController.gridNodeSize.y;
        jobData.startIndex = PybUtility.IndexTo1D(gridController.WorldToGridPoint(transform.position), gridController.gridNodeSize.x);
        jobData.targetIndex = PybUtility.IndexTo1D(gridController.WorldToGridPoint(playerTransform.position), gridController.gridNodeSize.x);
        NativeArray<AStarNode> grid = new NativeArray<AStarNode>(gridController.flatGrid.Length, Allocator.TempJob);
        jobData.grid = grid;
        jobData.grid.CopyFrom(gridController.flatGrid);
        jobData.pathPositions = result;

        JobHandle handle = jobData.Schedule();
        handle.Complete();

        enemyMovement.ClearPath();
        for (int i = 0; i < result.Length; i++)
        {
            enemyMovement.AddPathPoint(result[i]);
        }

        result.Dispose();
        grid.Dispose();
    }


    private bool IsPlayerPathClear()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);
        float maxFollowDistance = 2.5f;   //Only checks if the path is clear when within this distance
        if (playerDistance < maxFollowDistance)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            // Does the ray intersect any objects excluding the player layer
            if (!Physics.Raycast(transform.position, direction, out RaycastHit hit, playerDistance, obstacleLayer))
            {
                //Obstacles in way
                return true;
            }
        }
        return false;
    }

}
