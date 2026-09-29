using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private int _currentHealth;

    [Header("Invulnerability")]
    [SerializeField] private float _invulDuration = 1.0f;
    private float _lastDamageTime = -Mathf.Infinity;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged; // (current, max)
    public UnityEvent<float> OnHealthPercentChanged; // (0-1)
    public UnityEvent OnDamaged;
    public UnityEvent OnHealed;
    public UnityEvent OnDeath;

    public bool IsInvulnerable => Time.time < _lastDamageTime + _invulDuration;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public float HealthPercent => _maxHealth > 0 ? (float)_currentHealth / _maxHealth : 0f;
    public bool IsAlive => _currentHealth > 0;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth, bool healToFull = false)
    {
        _maxHealth = Mathf.Max(1, newMaxHealth);
        if (healToFull)
        {
            _currentHealth = _maxHealth;
        }
        else
        {
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        }
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnHealthPercentChanged?.Invoke(HealthPercent);
    }

    public void TakeDamage(int amount)
    {
        if (IsInvulnerable || _currentHealth <= 0) return;

        _lastDamageTime = Time.time;
        _currentHealth = Math.Max(0, _currentHealth - amount);

        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnHealthPercentChanged?.Invoke(HealthPercent);

        if (_currentHealth == 0)
        {
            Die();
        }
    }

    public bool Heal(int amount)
    {
        if (_currentHealth >= _maxHealth || _currentHealth <= 0) return false;

        _currentHealth = Math.Min(_maxHealth, _currentHealth + amount);
        OnHealed?.Invoke();
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnHealthPercentChanged?.Invoke(HealthPercent);
        return true;
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}