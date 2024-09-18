using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerController : Character, IPausable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform playerBodyTransform;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private AxeController axeController;

    private Vector3 movementVector;
    private Weapon currentMeleeWeapon;
    private bool attackingWithMeleeWeapon;
    private bool playerPaused;

    private void Awake()
    {
        Init(maxHealth, movementSpeed); // SO for player to be made later
        axeController = Instantiate(axeController);
        axeController.InitializeWeaponPositionAndOrientation(transform, playerBodyTransform);
        currentMeleeWeapon = axeController;
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
    }

    public void Resume()
    {
        playerPaused = false;
    }

    public void TakePlayerMovementInput(InputAction.CallbackContext ctx)
    {
        if (playerPaused) return;
        Vector2 playerInput = ctx.ReadValue<Vector2>();
        movementVector = new Vector3(playerInput.x, playerInput.y, 0f) * movementSpeed;
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

    private void MovePlayer() => transform.position += movementVector * Time.deltaTime;
    private void AttackWithMeleeWeapon()
    {
        if (attackingWithMeleeWeapon && !playerPaused)
            currentMeleeWeapon.Attack();
    }
}
