using System;

namespace UnityGameSystems.Progression
{
    public sealed class ExperienceProgression
    {
        private readonly Func<int, int> requiredXpForLevel;

        public int Level { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int TotalXp { get; private set; }

        public int RequiredXp => Math.Max(1, requiredXpForLevel(Level));
        public float NormalizedProgress => Math.Min(1f, CurrentXp / (float)RequiredXp);

        public event Action<int> LevelChanged;
        public event Action<int, int> ExperienceChanged;

        public ExperienceProgression(Func<int, int> requiredXpForLevel)
        {
            this.requiredXpForLevel = requiredXpForLevel ?? throw new ArgumentNullException(nameof(requiredXpForLevel));
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            TotalXp += amount;
            CurrentXp += amount;

            while (CurrentXp >= RequiredXp)
            {
                CurrentXp -= RequiredXp;
                Level++;
                LevelChanged?.Invoke(Level);
            }

            ExperienceChanged?.Invoke(CurrentXp, RequiredXp);
        }

        public void SetState(int level, int currentXp, int totalXp)
        {
            Level = Math.Max(1, level);
            CurrentXp = Math.Max(0, currentXp);
            TotalXp = Math.Max(CurrentXp, totalXp);

            while (CurrentXp >= RequiredXp)
            {
                CurrentXp -= RequiredXp;
                Level++;
            }

            ExperienceChanged?.Invoke(CurrentXp, RequiredXp);
            LevelChanged?.Invoke(Level);
        }
    }
}
