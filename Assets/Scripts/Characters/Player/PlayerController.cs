using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerController : Character
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform playerBodyTransform;
    [SerializeField] private AxeController axeController;

    private Vector3 movementVector;
    private Weapon currentMeleeWeapon;
    private bool attackingWithMeleeWeapon;

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
        AttackWithPrimaryWeapon();
    }

    public void TakePlayerMovementInput(InputAction.CallbackContext ctx)
    {
        Vector2 playerInput = ctx.ReadValue<Vector2>();
        movementVector = new Vector3(playerInput.x, playerInput.y, 0f) * movementSpeed;
        ChangeDirection(playerInput.x);
    }

    public Transform GetPlayerBodyTransform() => playerBodyTransform;

    public void TakePlayerAttackInput(InputAction.CallbackContext ctx)
    {    
        if(ctx.ReadValue<float>() > 0f)
            attackingWithMeleeWeapon = true;
        else
            attackingWithMeleeWeapon = false;
    }

    private void ChangeDirection(float input)
    {
        if (input > 0f)
            playerBodyTransform.eulerAngles = new Vector3(0f, 180f, 0f);
        else if (input < 0f)
            playerBodyTransform.eulerAngles = Vector3.zero;
    }

    private void MovePlayer() => transform.position += movementVector * Time.deltaTime;
    private void AttackWithPrimaryWeapon()
    {
        if (attackingWithMeleeWeapon)
            currentMeleeWeapon.Attack();
    }

    public override void TakeDamage(int someDamage, float knockBackForce)
    {
        base.TakeDamage(someDamage, knockBackForce);
    }
}
