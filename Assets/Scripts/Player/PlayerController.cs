using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform playerBodyTransform;
    [SerializeField] private Transform primaryWeaponTransform;
    [SerializeField] private float rotSpeed;

    private Vector3 movementVector;
    private bool attackingWithPrimaryWeapon;

    // Start is called before the first frame update
    void Start()
    {
        attackingWithPrimaryWeapon = false;
    }

    // Update is called once per frame
    void Update()
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

    public void TakePlayerAttackInput(InputAction.CallbackContext ctx)
    {    
        if(ctx.ReadValue<float>() > 0f)
            attackingWithPrimaryWeapon = true;
        else
            attackingWithPrimaryWeapon = false;
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
        if(attackingWithPrimaryWeapon)
            primaryWeaponTransform.RotateAround(playerBodyTransform.position, Vector3.forward, rotSpeed * Time.deltaTime);
    }
}
