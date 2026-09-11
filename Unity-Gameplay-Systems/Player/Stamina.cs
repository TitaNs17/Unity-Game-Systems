using System;
using UnityEngine;

namespace UnityGameSystems.Player
{
    public sealed class Stamina : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxStamina = 100f;
        [SerializeField, Min(0f)] private float regenerationPerSecond = 15f;
        [SerializeField, Min(0f)] private float regenerationDelay = 1f;

        private float lastSpendTime;

        public float Current { get; private set; }
        public float Max => maxStamina;
        public float Normalized => maxStamina <= 0f ? 0f : Current / maxStamina;

        public event Action<float, float> Changed;

        private void Awake()
        {
            Current = maxStamina;
        }

        private void Update()
        {
            if (Current >= maxStamina) return;
            if (Time.time < lastSpendTime + regenerationDelay) return;

            SetCurrent(Current + regenerationPerSecond * Time.deltaTime);
        }

        public bool CanSpend(float amount)
        {
            return amount > 0f && Current >= amount;
        }

        public bool TrySpend(float amount)
        {
            if (!CanSpend(amount)) return false;

            lastSpendTime = Time.time;
            SetCurrent(Current - amount);
            return true;
        }

        public void Restore(float amount)
        {
            if (amount <= 0f) return;
            SetCurrent(Current + amount);
        }

        public void RestoreFull()
        {
            SetCurrent(maxStamina);
        }

        private void SetCurrent(float value)
        {
            var next = Mathf.Clamp(value, 0f, maxStamina);
            if (Mathf.Approximately(next, Current)) return;

            Current = next;
            Changed?.Invoke(Current, maxStamina);
        }
    }
}
