using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SinusoidalMovement : EnemyMovement
{
    private const float sinAmplitude = 2f;
    private const float sinFrequency = 3.5f;
    public SinusoidalMovement(Transform playerTransform, Transform enemyTransform, float currentEnemySpeed)
    {
        this.playerTransform = playerTransform;
        this.enemyTransform = enemyTransform;
        SetCurrentEnemySpeed(currentEnemySpeed);
    }
    public override void UpdatePosition()
    {
        GetDirectionToPlayerVector();
        Vector3 horizontalMovement = currentEnemySpeed * enemySpeedModifier * Time.deltaTime * DirectionToPlayer.normalized;
        Vector3 perpendicularMovement = new Vector3(-horizontalMovement.y, horizontalMovement.x, 0);
        perpendicularMovement *= Mathf.Sin(Time.time * sinFrequency + sinusoidalOffset) *  sinAmplitude;
        enemyTransform.position += horizontalMovement + perpendicularMovement;
    }
}
