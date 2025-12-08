using System;

public class HealthModel
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0;
    
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public HealthModel(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        CurrentHealth -= damage;
        if (CurrentHealth < 0) CurrentHealth = 0;

        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}
