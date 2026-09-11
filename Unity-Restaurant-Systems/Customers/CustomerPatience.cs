using System;
using UnityEngine;

namespace UnityGameSystems.Restaurant
{
    public sealed class CustomerPatience : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxWaitTime = 60f;
        [SerializeField] private AnimationCurve satisfactionCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private float elapsed;
        private bool running;

        public float NormalizedWait => Mathf.Clamp01(elapsed / maxWaitTime);
        public float Satisfaction => Mathf.Clamp01(satisfactionCurve.Evaluate(NormalizedWait));
        public bool HasExpired => elapsed >= maxWaitTime;

        public event Action Expired;

        private void Update()
        {
            if (!running) return;

            elapsed += Time.deltaTime;
            if (!HasExpired) return;

            running = false;
            Expired?.Invoke();
        }

        public void StartWaiting()
        {
            elapsed = 0f;
            running = true;
        }

        public void StopWaiting()
        {
            running = false;
        }
    }
}
