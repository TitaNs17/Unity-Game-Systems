using System;
using UnityEngine;

namespace UnityGameSystems.Loot
{
    [Serializable]
    public sealed class LootEntry
    {
        public GameObject prefab;
        [Min(0f)] public float weight = 1f;
        [Min(1)] public int minAmount = 1;
        [Min(1)] public int maxAmount = 1;
    }

    [CreateAssetMenu(menuName = "Game/Loot Table", fileName = "LootTable")]
    public sealed class LootTable : ScriptableObject
    {
        [SerializeField] private LootEntry[] entries = Array.Empty<LootEntry>();
        [SerializeField, Range(0f, 1f)] private float dropChance = 1f;

        public bool TryRoll(out LootEntry entry, out int amount)
        {
            entry = null;
            amount = 0;

            if (entries == null || entries.Length == 0 || UnityEngine.Random.value > dropChance)
                return false;

            float totalWeight = 0f;
            foreach (var candidate in entries)
            {
                if (candidate != null && candidate.prefab != null && candidate.weight > 0f)
                    totalWeight += candidate.weight;
            }

            if (totalWeight <= 0f)
                return false;

            var roll = UnityEngine.Random.value * totalWeight;
            foreach (var candidate in entries)
            {
                if (candidate == null || candidate.prefab == null || candidate.weight <= 0f)
                    continue;

                roll -= candidate.weight;
                if (roll > 0f)
                    continue;

                entry = candidate;
                var min = Mathf.Max(1, candidate.minAmount);
                var max = Mathf.Max(min, candidate.maxAmount);
                amount = UnityEngine.Random.Range(min, max + 1);
                return true;
            }

            return false;
        }
    }
}
