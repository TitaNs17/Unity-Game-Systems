#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.Restaurant;

namespace UnityGameSystems.Tests
{
    public sealed class RestaurantEconomyTests
    {
        [Test]
        public void RevenueAndExpensesUpdateBalance()
        {
            var economy = new RestaurantEconomy(100m);

            economy.AddRevenue(50m);
            var spent = economy.TrySpend(30m);

            Assert.IsTrue(spent);
            Assert.AreEqual(120m, economy.Balance);
            Assert.AreEqual(50m, economy.Revenue);
            Assert.AreEqual(30m, economy.Expenses);
            Assert.AreEqual(20m, economy.Profit);
        }

        [Test]
        public void CannotSpendMoreThanBalance()
        {
            var economy = new RestaurantEconomy(25m);
            Assert.IsFalse(economy.TrySpend(30m));
            Assert.AreEqual(25m, economy.Balance);
        }
    }
}
#endif
