using UnityEngine;
using UnityEngine.UI;
using UnityGameSystems.Combat;

namespace UnityGameSystems.UI
{
    public sealed class HealthBarBinder : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Slider slider;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool hideWhenFull;

        private void Awake()
        {
            if (slider == null)
                slider = GetComponent<Slider>();

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (health != null)
                health.HealthChanged += Refresh;

            Refresh(health != null ? health.Current : 0f, health != null ? health.Max : 1f);
        }

        private void OnDisable()
        {
            if (health != null)
                health.HealthChanged -= Refresh;
        }

        public void Bind(Health target)
        {
            if (health != null)
                health.HealthChanged -= Refresh;

            health = target;

            if (isActiveAndEnabled && health != null)
                health.HealthChanged += Refresh;

            Refresh(health != null ? health.Current : 0f, health != null ? health.Max : 1f);
        }

        private void Refresh(float current, float max)
        {
            if (slider == null) return;

            slider.minValue = 0f;
            slider.maxValue = Mathf.Max(1f, max);
            slider.value = Mathf.Clamp(current, 0f, slider.maxValue);

            if (canvasGroup != null)
                canvasGroup.alpha = hideWhenFull && current >= max ? 0f : 1f;
        }
    }
}
