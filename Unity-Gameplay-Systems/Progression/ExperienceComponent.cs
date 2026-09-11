using System;
using UnityEngine;

namespace UnityGameSystems.Progression
{
    [DisallowMultipleComponent]
    public sealed class ExperienceComponent : MonoBehaviour
    {
        [SerializeField, Min(1)] private int baseRequiredXp = 100;
        [SerializeField, Min(1f)] private float growthMultiplier = 1.25f;

        private ExperienceProgression progression;

        public int Level => progression?.Level ?? 1;
        public int CurrentXp => progression?.CurrentXp ?? 0;
        public int RequiredXp => progression?.RequiredXp ?? baseRequiredXp;
        public float NormalizedProgress => progression?.NormalizedProgress ?? 0f;

        public event Action<int> LevelChanged;
        public event Action<int, int> ExperienceChanged;

        private void Awake()
        {
            progression = new ExperienceProgression(RequiredForLevel);
            progression.LevelChanged += level => LevelChanged?.Invoke(level);
            progression.ExperienceChanged += (current, required) => ExperienceChanged?.Invoke(current, required);
        }

        public void AddExperience(int amount)
        {
            progression?.AddExperience(amount);
        }

        public void RestoreState(int level, int currentXp, int totalXp)
        {
            progression?.SetState(level, currentXp, totalXp);
        }

        private int RequiredForLevel(int level)
        {
            var scaled = baseRequiredXp * Mathf.Pow(growthMultiplier, Mathf.Max(0, level - 1));
            return Mathf.Max(1, Mathf.RoundToInt(scaled));
        }
    }
}
