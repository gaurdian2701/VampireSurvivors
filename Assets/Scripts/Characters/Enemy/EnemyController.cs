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
    [SerializeField] private CircleCollider2D enemyHitBox;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private SpriteRenderer bloodSplatterSpriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    
    public EnemyMovement movementController;
    
    private Transform playerTransform;
    
    private bool isInKnockBack;
    private bool objectIsDisabled;
    private bool collidedWithPlayer;
    private bool enemyPaused;
    
    private float enemySpeedModifier;
    private float stoppingDistance;
    
    private int enemyDamage;
    
    private Color originalEnemyColor;
    private readonly Color enemyColorOnHit = new Color(205f, 0f, 0f);
    
    private const float knockBackDuration = 2.5f;
    private const int milliseconds = 100;
    private const int playerLevelMilestoneForIncreasingStats = 5;
    
    private void Awake()
    {
        Init(enemyData.EnemyMaxHealth, enemyData.EnemySpeed);
        stoppingDistance = enemyData.EnemyStoppingDistance;
        enemyDamage = enemyData.EnemyDamage;
        enemySpriteRenderer.sprite = enemyData.EnemySprite;
        playerTransform = GameManager.Instance.PlayerController.transform;
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
        enemyHitBox.enabled = true;
    }

    private void OnDisable()
    {
        objectIsDisabled = true;
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerLevelledUp += CheckForStatIncrease;
        GameManager.Instance.EventService.OnGameEnteredPauseState += Pause;
        GameManager.Instance.EventService.OnGameEnteredPlayState += Resume;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerLevelledUp -= CheckForStatIncrease;
        GameManager.Instance.EventService.OnGameEnteredPauseState -= Pause;
        GameManager.Instance.EventService.OnGameEnteredPlayState -= Resume;
    }
    public void InitializeEnemyData(EnemyScriptableObject enemyData)
    {
        this.enemyData = enemyData;
    }
    public Sprite GetEnemySprite() => enemyData.EnemySprite;

    private void CheckForStatIncrease()
    {
        if (GameManager.Instance.PlayerController.CurrentPlayerLevel % playerLevelMilestoneForIncreasingStats != 0)
            return;
        
        HealthController.SetMaxHealth(MaxHealth + enemyData.EnemyMaxHealthStatIncreaseRate);
        enemySpeedModifier += enemyData.EnemySpeedStatIncreaseRate;
        enemyDamage += enemyData.DamageStatIncreaseRate;
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
            other.GetComponent<PlayerController>().TakeDamage(enemyDamage);
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

    public override void Die()
    {
        if (objectIsDisabled)
            return;
        
        animator.enabled = true;
        bloodSplatterSpriteRenderer.enabled = true;
        enemySpriteRenderer.enabled = false;
        enemyHitBox.enabled = false;
        GameManager.Instance.EventService.InvokeEnemyDiedEvent(transform.position);
    }

    public void OnDied()
    {
        ReturnEnemyToPool();
        gameObject.SetActive(false);
    }

    private void ReturnEnemyToPool()
    {
        GameManager.Instance.ObjectPoolingService.MainEnemyPool.ReturnObjectToPool(this);
    }
}
