#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.Restaurant.Shift;
using UnityGameSystems.Restaurant.Stock;

namespace UnityGameSystems.Restaurant.Tests
{
    public sealed class StockAndShiftTests
    {
        [Test]
        public void IngredientStockBlocksOverConsumption()
        {
            var stock = new IngredientStock("tomato", 3, 1, 2m);

            Assert.IsFalse(stock.TryConsume(4));
            Assert.AreEqual(3, stock.Quantity);
            Assert.IsTrue(stock.TryConsume(2));
            Assert.IsTrue(stock.NeedsReorder);
        }

        [Test]
        public void ShiftSummaryCalculatesNetProfit()
        {
            var tracker = new ShiftTracker();
            tracker.RecordOrderReceived();
            tracker.RecordOrderCompleted(120m, 2);
            tracker.RecordExpense(30m);
            tracker.RecordWaste(10m);

            Assert.AreEqual(80m, tracker.Summary.NetProfit);
            Assert.AreEqual(2, tracker.Summary.CustomersServed);
            Assert.AreEqual(1f, tracker.Summary.CompletionRate);
        }
    }
}
#endif
