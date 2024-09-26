using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : Weapon
{
    [SerializeField] private WeaponScriptableObject weaponData;
    [SerializeField] private SpriteRenderer weaponImage;

    private void Awake()
    {
        InitWeaponData(weaponData);
        SubscribeToEvents();
    }

    ~AxeController()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameManager.Instance.EventService.OnPlayerPressedAttackButton += Attack;
    }

    public void UnsubscribeFromEvents()
    {
        GameManager.Instance.EventService.OnPlayerPressedAttackButton -= Attack;
    }

    public override void Attack()
    {
        transform.RotateAround(playerTransform.position, Vector3.forward, BaseAttackSpeed * Time.deltaTime);
    }
}
