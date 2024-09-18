using UnityEngine;

public class Character : MonoBehaviour
{
    public HealthController HealthController;
    public float MaxSpeed { get; private set; }

    public void Init(int maxHealth, float maxSpeed)
    {
        HealthController = new HealthController(maxHealth, this);
        MaxSpeed = maxSpeed;
    }
    public virtual void TakeDamage(int someDamage, float knockBackForce)
    {
        HealthController.DecreaseHealth(someDamage);
    }

    public virtual void TakeDamage(int someDamage)
    {
        HealthController.DecreaseHealth(someDamage);
    }

    public virtual void Die() { }
}
