using System;
using System.Collections.Generic;

namespace UnityGameSystems.Restaurant.Stock
{
    public sealed class WasteTracker
    {
        private readonly Dictionary<string, int> wastedUnits = new();

        public decimal WasteCost { get; private set; }

        public void Record(string ingredientId, int amount, decimal unitCost)
        {
            if (string.IsNullOrWhiteSpace(ingredientId) || amount <= 0) return;

            wastedUnits.TryGetValue(ingredientId, out var current);
            wastedUnits[ingredientId] = current + amount;
            WasteCost += Math.Max(0m, unitCost) * amount;
        }

        public int GetWastedUnits(string ingredientId)
        {
            return wastedUnits.TryGetValue(ingredientId, out var amount) ? amount : 0;
        }

        public void Reset()
        {
            wastedUnits.Clear();
            WasteCost = 0m;
        }
    }
}
