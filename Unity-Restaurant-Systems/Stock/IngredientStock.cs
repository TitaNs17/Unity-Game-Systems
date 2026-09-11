using System;

namespace UnityGameSystems.Restaurant.Stock
{
    public sealed class IngredientStock
    {
        public string IngredientId { get; }
        public int Quantity { get; private set; }
        public int ReorderLevel { get; }
        public decimal UnitCost { get; }

        public bool NeedsReorder => Quantity <= ReorderLevel;

        public IngredientStock(string ingredientId, int quantity, int reorderLevel, decimal unitCost)
        {
            IngredientId = ingredientId ?? string.Empty;
            Quantity = Math.Max(0, quantity);
            ReorderLevel = Math.Max(0, reorderLevel);
            UnitCost = Math.Max(0m, unitCost);
        }

        public void Add(int amount)
        {
            if (amount > 0)
                Quantity += amount;
        }

        public bool TryConsume(int amount)
        {
            if (amount <= 0 || Quantity < amount)
                return false;

            Quantity -= amount;
            return true;
        }
    }
}
