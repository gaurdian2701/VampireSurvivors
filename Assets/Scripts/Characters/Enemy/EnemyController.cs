using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Threading.Tasks;
public class EnemyController : Character
{
    [SerializeField] private EnemyScriptableObject enemyData;

    [Header("ENEMY COMPONENTS")]
    [SerializeField] private Transform enemyBodyTransform;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    private Transform playerTransform;
    private Vector3 directionToPlayer;
    private bool isInKnockBack = false;

    private float currentEnemySpeed;
    private Color originalEnemyColor;

    private static float knockBackDuration = 0.2f;
    private int millisecondsPerSecond = 1000;
    private Color enemyColorOnHit = new Color(205f, 0f, 0f);

    private void Awake()
    {
        Init(enemyData.MaxHealth, enemyData.EnemySpeed);
        currentEnemySpeed = MaxSpeed;
        originalEnemyColor = enemySpriteRenderer.color;
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
        if (!isInKnockBack)
            Async_InitiateKnockBack(knockBackForce);

        base.TakeDamage(someDamage, knockBackForce);
    }

    private async void Async_InitiateKnockBack(float knockBackForce)
    {
        isInKnockBack = true;
        currentEnemySpeed = 0f;
        enemySpriteRenderer.color = enemyColorOnHit;
        Vector2 knockBackDirection = transform.position - playerTransform.position;
        rb.AddForce(knockBackDirection.normalized * knockBackForce, ForceMode2D.Impulse);

        await Task.Delay((int)Math.Abs(knockBackDuration * millisecondsPerSecond));

        NullifyVelocities();
        currentEnemySpeed = MaxSpeed;
        enemySpriteRenderer.color = originalEnemyColor;
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

    public override void Die()
    {
        GameManager.Instance.EventService.InvokeEnemyDiedEvent();
        Destroy(gameObject);
    }
}
