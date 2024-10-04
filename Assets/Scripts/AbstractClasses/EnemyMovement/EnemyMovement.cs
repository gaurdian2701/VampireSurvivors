using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement
{
    public Vector3 DirectionToPlayer;
    
    protected float currentEnemySpeed;
    protected float enemySpeedModifier;
    protected Transform playerTransform;
    protected Transform enemyTransform;
    protected float sinusoidalOffset;

    public EnemyMovement()
    {
        SetEnemySpeedModifier(1f);
        SetEnemySinusoidalOffset(0f);
    }

    public abstract void UpdatePosition();
    public void SetCurrentEnemySpeed(float someSpeed) => currentEnemySpeed = someSpeed;
    public void SetEnemySpeedModifier(float someModifier) => enemySpeedModifier = someModifier;
    public void SetEnemySinusoidalOffset(float someOffset) => sinusoidalOffset = someOffset;

    protected Vector3 GetDirectionToPlayerVector()
    {
        DirectionToPlayer = playerTransform.position - enemyTransform.position;
        DirectionToPlayer.z = 0;
        return DirectionToPlayer;
    }
}
