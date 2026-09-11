namespace UnityGameSystems.Restaurant.Shift
{
    public sealed class ShiftTracker
    {
        private readonly ShiftSummary summary = new();

        public ShiftSummary Summary => summary;

        public void RecordOrderReceived() => summary.OrdersReceived++;
        public void RecordOrderCompleted(decimal revenue, int customers = 1)
        {
            summary.OrdersCompleted++;
            summary.CustomersServed += customers > 0 ? customers : 0;
            summary.Revenue += revenue > 0m ? revenue : 0m;
        }

        public void RecordOrderCancelled() => summary.OrdersCancelled++;

        public void RecordExpense(decimal amount)
        {
            if (amount > 0m)
                summary.Expenses += amount;
        }

        public void RecordWaste(decimal amount)
        {
            if (amount > 0m)
                summary.WasteCost += amount;
        }
    }
}
