using System.Collections.Generic;

namespace UnityGameSystems.Restaurant.Seating
{
    public sealed class SeatingService
    {
        private readonly List<RestaurantTable> tables = new();

        public IReadOnlyList<RestaurantTable> Tables => tables;

        public void Register(RestaurantTable table)
        {
            if (table != null && !tables.Contains(table))
                tables.Add(table);
        }

        public RestaurantTable FindBestTable(int partySize)
        {
            RestaurantTable best = null;
            var bestWaste = int.MaxValue;

            foreach (var table in tables)
            {
                if (table.IsReserved || table.AvailableSeats < partySize) continue;

                var waste = table.AvailableSeats - partySize;
                if (waste >= bestWaste) continue;

                best = table;
                bestWaste = waste;
            }

            return best;
        }

        public bool TrySeatParty(int partySize, out RestaurantTable table)
        {
            table = FindBestTable(partySize);
            return table != null && table.TrySeat(partySize);
        }
    }
}
