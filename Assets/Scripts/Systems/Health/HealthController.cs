using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController
{
    private int maxHealth;

    public int CurrentHealth { get; private set; }

    public HealthController(int maxHealth)
    {
        this.maxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void DecreaseHealth(int someHealth) => CurrentHealth -= someHealth;
}
