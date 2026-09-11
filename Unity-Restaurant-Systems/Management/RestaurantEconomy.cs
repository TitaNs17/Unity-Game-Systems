using System;

namespace UnityGameSystems.Restaurant
{
    public sealed class RestaurantEconomy
    {
        public decimal Balance { get; private set; }
        public decimal Revenue { get; private set; }
        public decimal Expenses { get; private set; }

        public event Action<decimal> BalanceChanged;

        public RestaurantEconomy(decimal startingBalance = 0m)
        {
            Balance = startingBalance;
        }

        public void AddRevenue(decimal amount)
        {
            if (amount <= 0m) return;
            Revenue += amount;
            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }

        public bool TrySpend(decimal amount)
        {
            if (amount <= 0m || amount > Balance) return false;
            Expenses += amount;
            Balance -= amount;
            BalanceChanged?.Invoke(Balance);
            return true;
        }

        public decimal Profit => Revenue - Expenses;
    }
}
