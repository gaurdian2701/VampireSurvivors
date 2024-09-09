using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float enemySpeed;
    [SerializeField] private Transform enemyBodyTransform;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.z = 0;
        transform.position += directionToPlayer.normalized * enemySpeed * Time.deltaTime;
        ChangeDirection(directionToPlayer);
    }

    private void ChangeDirection(Vector3 direction)
    {
        float dotProduct = Vector3.Dot(direction, enemyBodyTransform.position);
        if (dotProduct < 0)
            enemyBodyTransform.eulerAngles = new Vector3(0f, 180f, 0f);
        else if(dotProduct > 0)
            enemyBodyTransform.eulerAngles = Vector3.zero;
    }
}
