using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public HealthController HealthController;

    public void Init(int maxHealth)
    {
        HealthController = new HealthController(maxHealth);
    }
    public virtual void TakeDamage(int someDamage)
    {
        HealthController.DecreaseHealth(someDamage);
    }
    public virtual void Die() { }
}
