using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Threading.Tasks;
public class EnemyController : Character
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float enemySpeed;
    [SerializeField] private Transform enemyBodyTransform;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private Rigidbody2D rb;

    private static float knockBackDuration = 0.2f;

    private Transform playerTransform;
    private Vector3 directionToPlayer;
    private bool isInKnockBack = false;
    private float currentEnemySpeed;

    private void Awake()
    {
        Init(maxHealth);
        currentEnemySpeed = enemySpeed;
    }

    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        CalculatePlayerDirectionVector();
        ChangeDirection(directionToPlayer.normalized);
    }
    private void FixedUpdate()
    {
        MoveEnemy();
    }
    private void MoveEnemy() =>
        transform.position += directionToPlayer.normalized * currentEnemySpeed * Time.deltaTime;

    private void CalculatePlayerDirectionVector()
    {
        directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.z = 0;
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
        if (!isInKnockBack)
            SlowKnockBack(knockBackForce);
    }

    private async void SlowKnockBack(float knockBackForce)
    {
        isInKnockBack = true;
        currentEnemySpeed = 0f;
        Vector2 knockBackDirection = transform.position - playerTransform.position;
        rb.AddForce(knockBackDirection.normalized * knockBackForce, ForceMode2D.Impulse);

        await Task.Delay((int)Math.Abs(knockBackDuration * 1000));

        NullifyVelocities();
        currentEnemySpeed = enemySpeed;
        isInKnockBack = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //When enemies collide into one another during knockback,
        //they start to gain velocity, which is undesired behaviour.
        //This did not work with OnCollisionEnter2D for some reason.
        NullifyVelocities();
    }
    private void NullifyVelocities()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
