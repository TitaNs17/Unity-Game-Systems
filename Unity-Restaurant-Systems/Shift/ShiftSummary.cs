namespace UnityGameSystems.Restaurant.Shift
{
    public sealed class ShiftSummary
    {
        public int OrdersReceived { get; internal set; }
        public int OrdersCompleted { get; internal set; }
        public int OrdersCancelled { get; internal set; }
        public int CustomersServed { get; internal set; }
        public decimal Revenue { get; internal set; }
        public decimal Expenses { get; internal set; }
        public decimal WasteCost { get; internal set; }

        public decimal NetProfit => Revenue - Expenses - WasteCost;
        public float CompletionRate => OrdersReceived <= 0 ? 1f : (float)OrdersCompleted / OrdersReceived;
    }
}
