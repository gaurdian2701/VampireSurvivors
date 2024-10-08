using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private const float speed = 5f;
    private int damage;
    private float knockback;
    private void Update()
    {
        transform.position += speed * Time.deltaTime * transform.up;
    }

    public void InitDamageData(int damage, float knockback)
    {
        this.damage = damage;
        this.knockback = knockback;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
            other.GetComponent<EnemyController>().TakeDamage(damage, knockback);
        GameManager.Instance.ObjectPoolingService.ProjectilePool.ReturnObjectToPool(this);
        gameObject.SetActive(false);
    }
    
}
