using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private Vector3[] path;
    [SerializeField] private float speed = 10f;
    private int currentPathIndex = 0;

    private void Update()
    {
        if (path != null && path.Length > 0 && currentPathIndex < path.Length)
        {
            MoveEnemy();
        }
    }

    public void SetPath(Vector3[] _path)
    {
        path = _path;
        currentPathIndex = 0;
    }

    private void MoveEnemy()
    {
        Vector3 direction = (path[currentPathIndex] - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        float distance = Vector3.Distance(transform.position, path[0]);
        if (distance <= 1f)
        {
            currentPathIndex += 1;
        }
    }
}
