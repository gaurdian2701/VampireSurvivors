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
    private Transform playerBodyTransform;
    private Vector3 directionToPlayer;
    private bool isInKnockBack = false;

    private float currentEnemySpeed;
    private float enemySpeedModifier;
    private float currentEnemySpeedModifier;
    private Color originalEnemyColor;

    private static float knockBackDuration = 0.2f;
    private int millisecondsPerSecond = 1000;
    private Color enemyColorOnHit = new Color(205f, 0f, 0f);

    private void Awake()
    {
        Init(enemyData.MaxHealth, enemyData.EnemySpeed);
        currentEnemySpeed = MaxSpeed;
        enemySpeedModifier = enemyData.EnemySpeedModifier;
        currentEnemySpeedModifier = enemySpeedModifier;
        originalEnemyColor = enemySpriteRenderer.color;
    }

    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;
        playerBodyTransform = GameManager.Instance.Player.GetPlayerBodyTransform();
    }

    private void OnEnable()
    {
        HealthController.ResetHealth();
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
        transform.position += directionToPlayer.normalized * currentEnemySpeed * currentEnemySpeedModifier * Time.deltaTime;

    private void CalculatePlayerDirectionVector()
    {
        directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.z = 0;
    }

    private void ChangeDirection(Vector3 direction)
    {
        if (transform.position.x < playerTransform.position.x)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        else
            transform.eulerAngles = Vector3.zero;

        if (Vector2.Dot(playerBodyTransform.right, transform.position - playerTransform.position) < 0f)
            currentEnemySpeedModifier = 1f;
        else
            currentEnemySpeedModifier = enemySpeedModifier;
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
        GameManager.Instance.ObjectPoolingService.MermanEnemyPool.ReturnObjectToPool(this);
        gameObject.SetActive(false);
    }
}
