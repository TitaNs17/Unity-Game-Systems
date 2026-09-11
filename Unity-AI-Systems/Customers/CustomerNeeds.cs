using System;
using UnityEngine;

namespace UnityGameSystems.AI.Customers
{
    [Serializable]
    public sealed class CustomerNeeds
    {
        [SerializeField, Range(0f, 100f)] private float hunger = 50f;
        [SerializeField, Range(0f, 100f)] private float comfort = 100f;
        [SerializeField, Range(0f, 100f)] private float patience = 100f;

        public float Hunger => hunger;
        public float Comfort => comfort;
        public float Patience => patience;
        public bool HasLostPatience => patience <= 0f;

        public event Action Changed;

        public void Tick(float deltaTime, float patienceDrainPerSecond, float hungerGainPerSecond)
        {
            if (deltaTime <= 0f) return;

            hunger = Mathf.Clamp(hunger + Mathf.Max(0f, hungerGainPerSecond) * deltaTime, 0f, 100f);
            patience = Mathf.Clamp(patience - Mathf.Max(0f, patienceDrainPerSecond) * deltaTime, 0f, 100f);
            Changed?.Invoke();
        }

        public void AddComfort(float amount)
        {
            comfort = Mathf.Clamp(comfort + amount, 0f, 100f);
            Changed?.Invoke();
        }

        public void Feed(float amount)
        {
            hunger = Mathf.Clamp(hunger - Mathf.Max(0f, amount), 0f, 100f);
            Changed?.Invoke();
        }

        public void Reset(float initialHunger = 50f)
        {
            hunger = Mathf.Clamp(initialHunger, 0f, 100f);
            comfort = 100f;
            patience = 100f;
            Changed?.Invoke();
        }
    }
}
