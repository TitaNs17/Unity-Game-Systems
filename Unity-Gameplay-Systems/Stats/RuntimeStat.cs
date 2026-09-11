using System;
using System.Collections.Generic;

namespace UnityGameSystems.Stats
{
    public enum ModifierMode
    {
        Flat,
        Percent
    }

    public readonly struct StatModifier
    {
        public readonly float Value;
        public readonly ModifierMode Mode;
        public readonly object Source;

        public StatModifier(float value, ModifierMode mode, object source = null)
        {
            Value = value;
            Mode = mode;
            Source = source;
        }
    }

    public sealed class RuntimeStat
    {
        private readonly List<StatModifier> modifiers = new();
        private float baseValue;

        public event Action<float> ValueChanged;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                if (Math.Abs(baseValue - value) < 0.0001f) return;
                baseValue = value;
                ValueChanged?.Invoke(Value);
            }
        }

        public float Value
        {
            get
            {
                var flat = 0f;
                var percent = 0f;

                foreach (var modifier in modifiers)
                {
                    if (modifier.Mode == ModifierMode.Flat)
                        flat += modifier.Value;
                    else
                        percent += modifier.Value;
                }

                return (baseValue + flat) * (1f + percent);
            }
        }

        public RuntimeStat(float baseValue)
        {
            this.baseValue = baseValue;
        }

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            ValueChanged?.Invoke(Value);
        }

        public int RemoveModifiersFrom(object source)
        {
            var removed = modifiers.RemoveAll(x => ReferenceEquals(x.Source, source));
            if (removed > 0)
                ValueChanged?.Invoke(Value);
            return removed;
        }
    }
}
