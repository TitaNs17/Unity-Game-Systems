using System;
using UnityEngine;

namespace UnityGameSystems.Restaurant
{
    public sealed class ReputationSystem
    {
        private float value;

        public float Value => value;
        public event Action<float> Changed;

        public ReputationSystem(float initialValue = 50f)
        {
            value = Mathf.Clamp(initialValue, 0f, 100f);
        }

        public void Add(float amount)
        {
            Set(value + amount);
        }

        public void Remove(float amount)
        {
            Set(value - amount);
        }

        public float CustomerFlowMultiplier()
        {
            return Mathf.Lerp(0.5f, 1.5f, value / 100f);
        }

        private void Set(float next)
        {
            var clamped = Mathf.Clamp(next, 0f, 100f);
            if (Mathf.Approximately(clamped, value)) return;
            value = clamped;
            Changed?.Invoke(value);
        }
    }
}
