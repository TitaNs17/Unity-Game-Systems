using System;
using UnityEngine;

namespace UnityGameSystems.AI
{
    [Serializable]
    public sealed class SuspicionMeter
    {
        [SerializeField, Min(0.01f)] private float risePerSecond = 35f;
        [SerializeField, Min(0.01f)] private float decayPerSecond = 15f;
        [SerializeField, Min(1f)] private float alertThreshold = 100f;

        public float Value { get; private set; }
        public float Normalized => Mathf.Clamp01(Value / alertThreshold);
        public bool IsAlerted => Value >= alertThreshold;

        public event Action<float> Changed;
        public event Action Alerted;

        public void Tick(bool hasStimulus, float deltaTime)
        {
            if (deltaTime <= 0f) return;

            var previousAlerted = IsAlerted;
            var delta = hasStimulus ? risePerSecond : -decayPerSecond;
            var next = Mathf.Clamp(Value + delta * deltaTime, 0f, alertThreshold);

            if (Mathf.Approximately(next, Value)) return;

            Value = next;
            Changed?.Invoke(Value);

            if (!previousAlerted && IsAlerted)
                Alerted?.Invoke();
        }

        public void Add(float amount)
        {
            if (amount <= 0f) return;
            var wasAlerted = IsAlerted;
            Value = Mathf.Clamp(Value + amount, 0f, alertThreshold);
            Changed?.Invoke(Value);
            if (!wasAlerted && IsAlerted)
                Alerted?.Invoke();
        }

        public void Reset()
        {
            if (Mathf.Approximately(Value, 0f)) return;
            Value = 0f;
            Changed?.Invoke(Value);
        }
    }
}
