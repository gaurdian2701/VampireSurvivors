using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class EnemyController : Character
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float enemySpeed;
    [SerializeField] private Transform enemyBodyTransform;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private Rigidbody2D rigidBody;

    private Transform playerTransform;
    private Vector3 directionToPlayer;
    private bool isInKnockBack;

    private static float knockBackDuration = 0.1f;

    private void Awake()
    {
        Init(maxHealth);
        isInKnockBack = false;
    }

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
        directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.z = 0;
        transform.position += enemySpeed * Time.deltaTime * directionToPlayer.normalized;
        ChangeDirection(directionToPlayer.normalized);
    }

    private void ChangeDirection(Vector3 direction)
    {
        float dotProduct = Vector3.Dot(transform.right, direction);
        if (dotProduct < 0)
            enemyBodyTransform.eulerAngles = new Vector3(0f, 180f, 0f);
        else if(dotProduct >= 0)
            enemyBodyTransform.eulerAngles = Vector3.zero;
    }

    public override void TakeDamage(int someDamage, float knockBackForce)
    {
        base.TakeDamage(someDamage, knockBackForce);
        rigidBody.AddForce(new Vector3(-directionToPlayer.x, directionToPlayer.y, 0f) * knockBackForce, ForceMode2D.Impulse);
        if(!isInKnockBack)
            StartCoroutine(SlowKnockBack());
    }

    private IEnumerator SlowKnockBack()
    {
        isInKnockBack = true;
        yield return new WaitForSecondsRealtime(knockBackDuration);
        Debug.Log("coroutine finished executing");
        rigidBody.velocity = Vector2.zero;
        isInKnockBack = false;
    }
}
