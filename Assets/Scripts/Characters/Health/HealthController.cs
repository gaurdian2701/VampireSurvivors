using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController
{
    private int maxHealth;
    private Character character;

    public int CurrentHealth { get; private set; }

    public HealthController(int maxHealth, Character character)
    {
        this.maxHealth = maxHealth;
        this.character = character;
        CurrentHealth = maxHealth;
    }

    public void DecreaseHealth(int someHealth)
    {
        CurrentHealth -= someHealth;

        if (CurrentHealth <= 0)
            character.Die();
    }
}
