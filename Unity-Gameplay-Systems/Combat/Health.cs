using System;
using UnityEngine;

namespace UnityGameSystems.Combat
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [SerializeField] private bool startFull = true;
        [SerializeField] private bool destroyOnDeath;
        [SerializeField, Min(0f)] private float destroyDelay;

        public float Current { get; private set; }
        public float Max => maxHealth;
        public bool IsDead => Current <= 0f;
        public float Normalized => maxHealth <= 0f ? 0f : Current / maxHealth;

        public event Action<float, float> HealthChanged;
        public event Action<float> Damaged;
        public event Action<float> Healed;
        public event Action Died;

        private void Awake()
        {
            Current = startFull ? maxHealth : Mathf.Clamp(Current, 0f, maxHealth);
        }

        public bool TakeDamage(float amount)
        {
            if (amount <= 0f || IsDead) return false;

            var previous = Current;
            Current = Mathf.Max(0f, Current - amount);
            var applied = previous - Current;

            Damaged?.Invoke(applied);
            HealthChanged?.Invoke(Current, maxHealth);

            if (!IsDead) return true;

            Died?.Invoke();
            if (destroyOnDeath)
                Destroy(gameObject, destroyDelay);

            return true;
        }

        public bool Heal(float amount)
        {
            if (amount <= 0f || IsDead || Current >= maxHealth) return false;

            var previous = Current;
            Current = Mathf.Min(maxHealth, Current + amount);
            var applied = Current - previous;

            Healed?.Invoke(applied);
            HealthChanged?.Invoke(Current, maxHealth);
            return true;
        }

        public void RestoreFull()
        {
            Current = maxHealth;
            HealthChanged?.Invoke(Current, maxHealth);
        }

        public void SetMaxHealth(float value, bool refill = false)
        {
            maxHealth = Mathf.Max(1f, value);
            Current = refill ? maxHealth : Mathf.Min(Current, maxHealth);
            HealthChanged?.Invoke(Current, maxHealth);
        }
    }
}
