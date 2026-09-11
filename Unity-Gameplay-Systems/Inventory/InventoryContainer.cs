using System;
using System.Collections.Generic;

namespace UnityGameSystems.Inventory
{
    [Serializable]
    public sealed class InventoryEntry
    {
        public ItemDefinition item;
        public int amount;

        public InventoryEntry(ItemDefinition item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    public sealed class InventoryContainer
    {
        private readonly List<InventoryEntry> entries;

        public IReadOnlyList<InventoryEntry> Entries => entries;
        public int Capacity { get; }
        public event Action Changed;

        public InventoryContainer(int capacity)
        {
            Capacity = Math.Max(1, capacity);
            entries = new List<InventoryEntry>(Capacity);
        }

        public int Add(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return amount;

            foreach (var entry in entries)
            {
                if (entry.item != item || entry.amount >= item.MaxStack) continue;
                var accepted = Math.Min(amount, item.MaxStack - entry.amount);
                entry.amount += accepted;
                amount -= accepted;
                if (amount == 0)
                {
                    Changed?.Invoke();
                    return 0;
                }
            }

            while (amount > 0 && entries.Count < Capacity)
            {
                var accepted = Math.Min(amount, item.MaxStack);
                entries.Add(new InventoryEntry(item, accepted));
                amount -= accepted;
            }

            Changed?.Invoke();
            return amount;
        }

        public bool Remove(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0 || Count(item) < amount) return false;

            for (var i = entries.Count - 1; i >= 0 && amount > 0; i--)
            {
                var entry = entries[i];
                if (entry.item != item) continue;

                var removed = Math.Min(amount, entry.amount);
                entry.amount -= removed;
                amount -= removed;

                if (entry.amount == 0)
                    entries.RemoveAt(i);
            }

            Changed?.Invoke();
            return true;
        }

        public int Count(ItemDefinition item)
        {
            var total = 0;
            foreach (var entry in entries)
                if (entry.item == item) total += entry.amount;
            return total;
        }
    }
}
