using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Threading.Tasks;
public class EnemyController : Character, IPausable
{
    [SerializeField] private EnemyScriptableObject enemyData;

    [Header("ENEMY COMPONENTS")]
    [SerializeField] private Transform enemyBodyTransform;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private SpriteRenderer bloodSplatterSpriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private EnemyMovement movementController;
    
    private Transform playerTransform;
    private Transform playerBodyTransform;
    
    private bool isInKnockBack;
    private bool objectIsDisabled;
    private bool collidedWithPlayer;
    private bool enemyPaused;
    
    private float enemySpeedModifier;
    private float stoppingDistance;
    private int enemyDamage;
    private Color originalEnemyColor;
    private EnemyClass enemyClass;

    
    private static float knockBackDuration = 2.5f;
    private static int milliseconds = 100;
    private static Color enemyColorOnHit = new Color(205f, 0f, 0f);

    private void Awake()
    {
        Init(enemyData.EnmeyMaxHealth, enemyData.EnemySpeed);
        enemySpeedModifier = enemyData.EnemySpeedModifier;
        stoppingDistance = enemyData.EnemyStoppingDistance;
        enemyDamage = enemyData.EnemyDamage;
        enemyClass = enemyData.EnemyClass;
        playerTransform = GameManager.Instance.PlayerController.transform;
        playerBodyTransform = GameManager.Instance.PlayerController.GetPlayerBodyTransform();
        LoadMovementController(enemyData.EnemyMovementType);
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void OnEnable()
    {
        HealthController.ResetHealth();
        animator.enabled = false;
        bloodSplatterSpriteRenderer.enabled = false;
        enemySpriteRenderer.enabled = true;
        objectIsDisabled = false;
    }

    private void OnDisable()
    {
        objectIsDisabled = true;
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState += Pause;
        GameManager.Instance.EventService.OnGameEnteredPlayState += Resume;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnGameEnteredPauseState -= Pause;
        GameManager.Instance.EventService.OnGameEnteredPlayState -= Resume;
    }

    public void Pause() => enemyPaused = true;

    public void Resume() => enemyPaused = false;

    private void LoadMovementController(EnemyMovementType movementType)
    {
        switch (movementType)
        {
            default:
            case EnemyMovementType.STANDARD:
                movementController = new StandardMovement(playerTransform, transform, MaxSpeed);
                break;
            
            case EnemyMovementType.SINUSOIDAL:
                movementController = new SinusoidalMovement(playerTransform, transform, MaxSpeed);
                break;
        }
    }

    private void Start()
    {
        originalEnemyColor = enemySpriteRenderer.color;
        animator.enabled = false;
        bloodSplatterSpriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (enemyPaused)
            return;
        
        MoveEnemy();
        UpdateSpriteDirection();
        CheckIfPlayerIsFacingEnemy();
        CheckStoppingDistance();
    }

    private void MoveEnemy() => movementController.UpdatePosition();

    private void CheckStoppingDistance()
    {
        if (movementController.DirectionToPlayer.magnitude < stoppingDistance)
            movementController.SetCurrentEnemySpeed(0f);
        else
            movementController.SetCurrentEnemySpeed(MaxSpeed);
    }

    private void UpdateSpriteDirection()
    {
        if (transform.position.x < playerTransform.position.x)
            enemySpriteRenderer.flipX = true;
        else
            enemySpriteRenderer.flipX = false;
    }

    private void CheckIfPlayerIsFacingEnemy() //Enemy moves faster if player is not facing enemy
    {
        if (Vector2.Dot(playerBodyTransform.right, transform.position - playerTransform.position) < 0f)
            movementController.SetEnemySpeedModifier(1f);
        else
            movementController.SetEnemySpeedModifier(enemySpeedModifier);
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
        movementController.SetCurrentEnemySpeed(0f);
        enemySpriteRenderer.color = enemyColorOnHit;
        Vector2 knockBackDirection = transform.position - playerTransform.position;
        rb.AddForce(knockBackDirection.normalized * knockBackForce, ForceMode2D.Impulse);

        await NullifyVelocities((int)Math.Abs(knockBackDuration * milliseconds));

        movementController.SetCurrentEnemySpeed(MaxSpeed);
        enemySpriteRenderer.color = originalEnemyColor;
        isInKnockBack = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       movementController.SetCurrentEnemySpeed(0f);
    }
    private async void OnCollisionExit2D(Collision2D collision)
    {
        //When enemies collide into one another during knockback,
        //they start to gain velocity, which is undesired behaviour.
        //This did not work with OnCollisionEnter2D for some reason.
        await NullifyVelocities((int)Math.Abs(knockBackDuration * milliseconds));
        movementController.SetCurrentEnemySpeed(MaxSpeed);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collidedWithPlayer)
        {
            GameManager.Instance.EventService.InvokePlayerTookDamageEvent(enemyDamage);
            collidedWithPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            collidedWithPlayer = false;
    }

    private async Task NullifyVelocities(int millisecs)
    {
        await Task.Delay(millisecs);

        if (objectIsDisabled)
            return;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public override void Die() => InitiateDeathAnimation();

    private void InitiateDeathAnimation()
    {
        animator.enabled = true;
        bloodSplatterSpriteRenderer.enabled = true;
        enemySpriteRenderer.enabled = false;    
    }

    public void OnDied()
    {
        GameManager.Instance.EventService.InvokeEnemyDiedEvent(enemyClass);
        ReturnEnemyToPool(enemyClass);
        gameObject.SetActive(false);
    }

    private void ReturnEnemyToPool(EnemyClass enemyClass)
    {
        switch (enemyClass)
        {
            case EnemyClass.MERMAN :
                GameManager.Instance.ObjectPoolingService.Merman_EnemyPool.ReturnObjectToPool(this);
                break;
            case EnemyClass.RAVEN :
                GameManager.Instance.ObjectPoolingService.Raven_EnemyPool.ReturnObjectToPool(this);
                break;
        }
    }
}
