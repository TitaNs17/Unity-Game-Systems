using UnityEngine;
using UnityGameSystems.Combat;

namespace UnityGameSystems.Loot
{
    [DisallowMultipleComponent]
    public sealed class LootDropper : MonoBehaviour
    {
        [SerializeField] private LootTable lootTable;
        [SerializeField] private Health health;
        [SerializeField] private Transform dropOrigin;
        [SerializeField, Min(0f)] private float scatterRadius = 0.4f;
        [SerializeField, Min(0f)] private float upwardImpulse = 1.5f;

        private bool dropped;

        private void Awake()
        {
            if (health == null)
                health = GetComponent<Health>();

            if (dropOrigin == null)
                dropOrigin = transform;
        }

        private void OnEnable()
        {
            if (health != null)
                health.Died += Drop;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= Drop;
        }

        public void Drop()
        {
            if (dropped || lootTable == null)
                return;

            dropped = true;

            if (!lootTable.TryRoll(out var entry, out var amount))
                return;

            for (var i = 0; i < amount; i++)
            {
                var offset2D = Random.insideUnitCircle * scatterRadius;
                var position = dropOrigin.position + new Vector3(offset2D.x, 0f, offset2D.y);
                var instance = Instantiate(entry.prefab, position, Random.rotation);
                var body = instance.GetComponent<Rigidbody>();

                if (body != null && upwardImpulse > 0f)
                    body.AddForce(Vector3.up * upwardImpulse, ForceMode.Impulse);
            }
        }
    }
}
