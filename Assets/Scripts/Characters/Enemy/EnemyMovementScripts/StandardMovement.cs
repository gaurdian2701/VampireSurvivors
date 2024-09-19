using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardMovement : EnemyMovement
{
    public StandardMovement(Transform playerTransform, Transform enemyTransform, float currentEnemySpeed)
    {
        this.playerTransform = playerTransform;
        this.enemyTransform = enemyTransform;
        SetCurrentEnemySpeed(currentEnemySpeed);
    }

    public override void UpdatePosition()
    {
        GetDirectionToPlayerVector();
        enemyTransform.position += currentEnemySpeed * enemySpeedModifier * Time.deltaTime * DirectionToPlayer.normalized; 
    }
}
