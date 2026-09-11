using UnityEngine;
using UnityGameSystems.Combat;

namespace UnityGameSystems.Progression
{
    [DisallowMultipleComponent]
    public sealed class ExperienceReward : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private ExperienceComponent receiver;
        [SerializeField, Min(0)] private int amount = 25;

        private bool granted;

        private void Awake()
        {
            if (health == null)
                health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health != null)
                health.Died += Grant;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= Grant;
        }

        public void SetReceiver(ExperienceComponent target)
        {
            receiver = target;
        }

        public void Grant()
        {
            if (granted || receiver == null || amount <= 0)
                return;

            granted = true;
            receiver.AddExperience(amount);
        }
    }
}
