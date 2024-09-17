using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidalMovement : EnemyMovement
{
    public SinusoidalMovement(Transform playerTransform, Transform enemyTransform, float currentEnemySpeed, float enemySpeedModifier)
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
        Vector3 horizontalMovement = currentEnemySpeed * enemySpeedModifier * Time.deltaTime * DirectionToPlayer.normalized;
        horizontalMovement.y += Mathf.Sin(Time.time * 5f) * 2f * Time.deltaTime;
        enemyTransform.position += horizontalMovement;
    }
}
