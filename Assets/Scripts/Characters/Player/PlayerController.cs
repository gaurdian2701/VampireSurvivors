using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerController : Character, IPausable
{
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    [SerializeField] private Transform playerBodyTransform;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private AxeController axeController;

    private Vector3 movementVector;
    private Weapon currentMeleeWeapon;
    private bool attackingWithMeleeWeapon;
    private bool playerPaused;
    private float currentSpeed;

    private void Awake()
    {
        Init(playerScriptableObject.PlayerMaxHealth, playerScriptableObject.PlayerMovementSpeed); // SO for player to be made later
        axeController = Instantiate(axeController);
        axeController.InitializeWeaponPositionAndOrientation(transform, playerBodyTransform);
        currentMeleeWeapon = axeController;
        currentSpeed = MaxSpeed;
    }
    private void Start()
    {
        attackingWithMeleeWeapon = false;
    }

    private void Update()
    {
        MovePlayer();
        AttackWithMeleeWeapon();
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

    public void TakePlayerMovementInput(InputAction.CallbackContext ctx)
    {
        Vector2 playerInput = ctx.ReadValue<Vector2>();
        movementVector = new Vector3(playerInput.x, playerInput.y, 0f);
        ChangeDirection(playerInput.x);
    }

    public Transform GetPlayerBodyTransform() => playerBodyTransform;

    public void GetPlayerMeleeAttackInput(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValue<float>() > 0f)
            attackingWithMeleeWeapon = true;
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
            playerSpriteRenderer.flipX = true;
        else if (input < 0f)
            playerSpriteRenderer.flipX = false;
    }
 
    private void MovePlayer() => transform.position += currentSpeed * Time.deltaTime * movementVector;
    private void AttackWithMeleeWeapon()
    {
        if (attackingWithMeleeWeapon && !playerPaused)
            currentMeleeWeapon.Attack();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pickup"))
        {
            GameObject xp = other.gameObject;
            xp.gameObject.SetActive(false);
            GameManager.Instance.ObjectPoolingService.XpPool.ReturnObjectToPool(xp);
        }
    }
}
