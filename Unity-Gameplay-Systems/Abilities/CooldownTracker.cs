using System.Collections.Generic;

namespace UnityGameSystems.Abilities
{
    public sealed class CooldownTracker
    {
        private readonly Dictionary<string, float> remaining = new();

        public bool IsReady(string id)
        {
            return !remaining.TryGetValue(id, out var time) || time <= 0f;
        }

        public float Remaining(string id)
        {
            return remaining.TryGetValue(id, out var time) ? time : 0f;
        }

        public bool TryStart(AbilityDefinition ability)
        {
            if (ability == null || !IsReady(ability.Id)) return false;
            remaining[ability.Id] = ability.Cooldown;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (remaining.Count == 0) return;

            var keys = new List<string>(remaining.Keys);
            foreach (var key in keys)
            {
                remaining[key] -= deltaTime;
                if (remaining[key] <= 0f)
                    remaining.Remove(key);
            }
        }

        public void Reset() => remaining.Clear();
    }
}
