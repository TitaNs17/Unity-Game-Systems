using UnityEngine;

namespace UnityGameSystems.Combat
{
    [DisallowMultipleComponent]
    public sealed class DamageDealer : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float damage = 10f;
        [SerializeField] private bool triggerDamage = true;
        [SerializeField] private bool collisionDamage;
        [SerializeField] private bool singleUse;
        [SerializeField] private LayerMask targetLayers = ~0;

        private bool consumed;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerDamage)
                TryApply(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collisionDamage)
                TryApply(collision.gameObject);
        }

        public bool TryApply(GameObject target)
        {
            if (target == null || consumed) return false;
            if ((targetLayers.value & (1 << target.layer)) == 0) return false;

            var health = target.GetComponentInParent<Health>();
            if (health == null || !health.TakeDamage(damage)) return false;

            if (singleUse)
            {
                consumed = true;
                gameObject.SetActive(false);
            }

            return true;
        }
    }
}
