using System;

namespace UnityGameSystems.Restaurant.Seating
{
    public sealed class RestaurantTable
    {
        public int Capacity { get; }
        public int OccupiedSeats { get; private set; }
        public bool IsReserved { get; private set; }
        public string ReservationId { get; private set; }

        public bool HasSpace => OccupiedSeats < Capacity;
        public int AvailableSeats => Capacity - OccupiedSeats;

        public RestaurantTable(int capacity)
        {
            Capacity = Math.Max(1, capacity);
        }

        public bool TryReserve(string reservationId)
        {
            if (IsReserved || OccupiedSeats > 0 || string.IsNullOrWhiteSpace(reservationId)) return false;
            IsReserved = true;
            ReservationId = reservationId;
            return true;
        }

        public bool TrySeat(int partySize, string reservationId = null)
        {
            if (partySize <= 0 || partySize > AvailableSeats) return false;
            if (IsReserved && ReservationId != reservationId) return false;

            OccupiedSeats += partySize;
            IsReserved = false;
            ReservationId = null;
            return true;
        }

        public void Clear()
        {
            OccupiedSeats = 0;
            IsReserved = false;
            ReservationId = null;
        }
    }
}
