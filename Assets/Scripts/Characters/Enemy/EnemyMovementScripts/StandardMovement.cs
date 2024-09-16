using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardMovement : EnemyMovement
{
    public StandardMovement(Transform playerTransform, Transform enemyTransform, float currentEnemySpeed, float enemySpeedModifier)
    {
        this.playerTransform = playerTransform;
        this.enemyTransform = enemyTransform;
        SetCurrentEnemySpeed(currentEnemySpeed);
        SetEnemySpeedModifier(enemySpeedModifier);
    }

    public override void UpdatePosition()
    {
        DirectionToPlayer = playerTransform.position - enemyTransform.position;
        DirectionToPlayer.z = 0;
        enemyTransform.position += currentEnemySpeed * enemySpeedModifier * Time.deltaTime * DirectionToPlayer.normalized; 
    }
}
