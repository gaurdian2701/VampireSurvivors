using UnityEngine;

public class Character : MonoBehaviour
{
    public HealthController HealthController;

    public void Init(int maxHealth)
    {
        HealthController = new HealthController(maxHealth);
    }
    public virtual void TakeDamage(int someDamage, float knockBackForce)
    {
        HealthController.DecreaseHealth(someDamage);
    }
    public virtual void Die() { }
}
