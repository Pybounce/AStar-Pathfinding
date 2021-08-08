using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private List<Vector3> path = new List<Vector3>();
    [SerializeField] private float speed = 10f;
    private int currentPathIndex = 0;

    private Transform playerTransform;

    public bool followPlayer = false;  //If false, it will follow the path instead

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        
        if (followPlayer == true)
        {
            //Path clear to player
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            transform.position += directionToPlayer * speed * Time.deltaTime;
        }
        else if (path != null && path.Count > 0 && currentPathIndex < path.Count)
        {
            MoveEnemy();
        }
    }

    public void ClearPath()
    {
        path.Clear();
    }
    public void AddPathPoint(Vector3 _position)
    {
        path.Add(_position);
        currentPathIndex = 0;
    }
    


    private void MoveEnemy()
    {
        
        Vector3 direction = (path[currentPathIndex] - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, path[currentPathIndex]);
        if (distance <= 0.1f)
        {
            currentPathIndex += 1;
        }
    }

    

}
