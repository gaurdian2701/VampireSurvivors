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

    public EnemyMovement()
    {
        SetEnemySpeedModifier(1f);
    }

    public abstract void UpdatePosition();
    public void SetCurrentEnemySpeed(float someSpeed) => currentEnemySpeed = someSpeed;
    public void SetEnemySpeedModifier(float someModifier) => enemySpeedModifier = someModifier;

    protected Vector3 GetDirectionToPlayerVector()
    {
        DirectionToPlayer = playerTransform.position - enemyTransform.position;
        DirectionToPlayer.z = 0;
        return DirectionToPlayer;
    }
}
