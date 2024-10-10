using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerController : Character, IPausable
{
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    [SerializeField] private PlayerXpControllerScriptableObject playerXpControllerScriptableObject;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Animator animator;

    private PlayerXpController playerXpController;
    private EventService eventService;
    private Vector3 movementVector;
    private Vector2 playerMovementInput;
    private bool attackingWithMeleeWeapon;
    private bool playerPaused;
    private float currentSpeed;
    
    public int CurrentPlayerLevel { get; private set; }

    private void Awake()
    {
        Init(playerScriptableObject.PlayerMaxHealth, playerScriptableObject.PlayerMovementSpeed);
        playerXpController = new PlayerXpController(playerXpControllerScriptableObject);
        currentSpeed = MaxSpeed;
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerLevelledUp += ResetPlayerAttributesOnLevelUp;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerLevelledUp -= ResetPlayerAttributesOnLevelUp;
    }
    private void Start()
    {
        attackingWithMeleeWeapon = false;
        eventService = GameManager.Instance.EventService;
    }

    private void Update()
    {
        MovePlayer();
        AttackWithMeleeWeapon();
    }

    private void ResetPlayerAttributesOnLevelUp()
    {
        HealthController.ResetHealth();
        CurrentPlayerLevel++;
    }
    public void Pause()
    {
        playerPaused = true;
        currentSpeed = 0f;
    }

    public void Resume()
    {
        playerPaused = false; 
        currentSpeed = MaxSpeed;
    }

    public int GetPlayerMaxHealth() => HealthController.GetMaxHealth();
    public int GetCurrentHealthOfPlayer() => HealthController.CurrentHealth;
    public int GetCurrentXpToNextLevel() => playerXpController.currentXpToNextLevel;
    public int GetCurrentPlayerXp() => playerXpController.currentXp;

    public void TakePlayerMovementInput(InputAction.CallbackContext ctx)
    {
        playerMovementInput = ctx.ReadValue<Vector2>();
        movementVector = new Vector3(playerMovementInput.x, playerMovementInput.y, 0f);
        ChangeDirection(playerMovementInput.x);
    }

    public void GetPlayerMeleeAttackInput(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0f)
        {
            attackingWithMeleeWeapon = true;
        }
        else
            attackingWithMeleeWeapon = false;
    }

    public override void TakeDamage(int damageTaken)
    {
        base.TakeDamage(damageTaken);
        GameManager.Instance.EventService.InvokePlayerTookDamageEvent(damageTaken);
    }

    private void ChangeDirection(float input)
    {
        if (input > 0f)
            playerSpriteRenderer.flipX = false;
        else if (input < 0f)
            playerSpriteRenderer.flipX = true;
    }

    private void MovePlayer()
    {
        transform.position += currentSpeed * Time.deltaTime * movementVector;
        animator.SetFloat("Speed", playerMovementInput.magnitude);
    }
    private void AttackWithMeleeWeapon()
    {
        if (attackingWithMeleeWeapon && !playerPaused)
            eventService.InvokePlayerPressedAttackButtonEvent();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pickup"))
        {
            GameObject xp = other.gameObject;
            GameManager.Instance.ObjectPoolingService.XpPool.ReturnObjectToPool(xp);
            GameManager.Instance.EventService.InvokePlayerPickedUpXpEvent();
            xp.SetActive(false);
        }
    }
}
