using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : Weapon
{
    [SerializeField] private WeaponScriptableObject weaponData;
    [SerializeField] private SpriteRenderer weaponImage;

    private Transform playerBodyTransform;

    private void Awake()
    {
        InitWeaponData(weaponData);
    }

    public void InitializeWeaponPositionAndOrientation(Transform playerTransform, Transform playerBodyTransform)
    {
        transform.SetParent(playerTransform);
        this.playerBodyTransform = playerBodyTransform;
        transform.localPosition = weaponData.LocalPosition;
        transform.eulerAngles = weaponData.LocalRotation;
        transform.localScale = weaponData.Scale;
        weaponImage.sprite = weaponData.WeaponImage;
    }

    public override void Attack()
    {
        transform.RotateAround(playerBodyTransform.position, Vector3.forward, baseAttackSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy))
            enemy.TakeDamage(baseDamage, knockBackForce);
    }
}
