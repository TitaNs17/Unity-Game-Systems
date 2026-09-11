using System;

namespace UnityGameSystems.Restaurant.Staff
{
    public enum StaffRole
    {
        Cashier,
        Cook,
        Server,
        Cleaner
    }

    public sealed class StaffMember
    {
        public string Name { get; }
        public StaffRole Role { get; }
        public float Skill { get; private set; }
        public float Energy { get; private set; } = 100f;
        public decimal WagePerShift { get; }

        public StaffMember(string name, StaffRole role, float skill, decimal wagePerShift)
        {
            Name = name;
            Role = role;
            Skill = Math.Clamp(skill, 0.5f, 2f);
            WagePerShift = Math.Max(0m, wagePerShift);
        }

        public float WorkSpeedMultiplier => Skill * (0.5f + Energy / 200f);

        public void Work(float effort)
        {
            if (effort <= 0f) return;
            Energy = Math.Max(0f, Energy - effort);
        }

        public void Rest(float amount)
        {
            if (amount <= 0f) return;
            Energy = Math.Min(100f, Energy + amount);
        }

        public void Train(float amount)
        {
            if (amount <= 0f) return;
            Skill = Math.Clamp(Skill + amount, 0.5f, 2f);
        }
    }
}
