using System;
using System.Collections.Generic;

namespace UnityGameSystems.Restaurant
{
    public enum RestaurantOrderState
    {
        Waiting,
        Preparing,
        Ready,
        Served,
        Cancelled
    }

    public sealed class RestaurantOrder
    {
        private readonly List<MenuItemDefinition> items;

        public string Id { get; }
        public IReadOnlyList<MenuItemDefinition> Items => items;
        public RestaurantOrderState State { get; private set; }
        public float CreatedAt { get; }
        public float TotalPrice { get; }

        public event Action<RestaurantOrderState> StateChanged;

        public RestaurantOrder(string id, IEnumerable<MenuItemDefinition> items, float createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
            this.items = new List<MenuItemDefinition>(items ?? Array.Empty<MenuItemDefinition>());

            float total = 0f;
            foreach (var item in this.items)
                if (item != null) total += item.Price;

            TotalPrice = total;
            State = RestaurantOrderState.Waiting;
        }

        public bool TrySetState(RestaurantOrderState next)
        {
            if (!IsValidTransition(State, next)) return false;
            State = next;
            StateChanged?.Invoke(State);
            return true;
        }

        private static bool IsValidTransition(RestaurantOrderState current, RestaurantOrderState next)
        {
            if (next == RestaurantOrderState.Cancelled)
                return current != RestaurantOrderState.Served && current != RestaurantOrderState.Cancelled;

            return (current, next) switch
            {
                (RestaurantOrderState.Waiting, RestaurantOrderState.Preparing) => true,
                (RestaurantOrderState.Preparing, RestaurantOrderState.Ready) => true,
                (RestaurantOrderState.Ready, RestaurantOrderState.Served) => true,
                _ => false
            };
        }
    }
}
