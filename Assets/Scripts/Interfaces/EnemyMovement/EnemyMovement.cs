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
    
    public abstract void UpdatePosition();
    public void SetCurrentEnemySpeed(float someSpeed) => currentEnemySpeed = someSpeed;
    public void SetEnemySpeedModifier(float someModifier) => enemySpeedModifier = someModifier; 
}
