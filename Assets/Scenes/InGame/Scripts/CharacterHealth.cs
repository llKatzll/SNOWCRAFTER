using UnityEngine;
using System;

public class CharacterHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 30f;

    private float currentHealth;
    private SnowBallThrower playerThrower;
    private EnemySnowBallAI enemyThrower;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnDamaged;

    private void Start()
    {
        currentHealth = maxHealth;
        playerThrower = GetComponent<SnowBallThrower>();
        enemyThrower = GetComponent<EnemySnowBallAI>();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamaged?.Invoke();

        if (playerThrower != null && playerThrower.IsCharging)
        {
            playerThrower.CancelCharge();
        }

        if (enemyThrower != null && enemyThrower.IsCharging)
        {
            enemyThrower.CancelCharge();
        }

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}