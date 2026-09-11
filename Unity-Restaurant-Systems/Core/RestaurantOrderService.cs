using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameSystems.Restaurant
{
    public sealed class RestaurantOrderService : MonoBehaviour
    {
        private readonly Dictionary<string, RestaurantOrder> orders = new();
        private readonly Queue<string> waitingQueue = new();

        public event Action<RestaurantOrder> OrderCreated;
        public event Action<RestaurantOrder> OrderUpdated;

        public IEnumerable<RestaurantOrder> ActiveOrders => orders.Values;

        public RestaurantOrder CreateOrder(IEnumerable<MenuItemDefinition> items)
        {
            var id = Guid.NewGuid().ToString("N");
            var order = new RestaurantOrder(id, items, Time.time);
            orders.Add(id, order);
            waitingQueue.Enqueue(id);
            order.StateChanged += _ => OrderUpdated?.Invoke(order);
            OrderCreated?.Invoke(order);
            return order;
        }

        public bool TryGetNextWaiting(out RestaurantOrder order)
        {
            while (waitingQueue.Count > 0)
            {
                var id = waitingQueue.Dequeue();
                if (!orders.TryGetValue(id, out order)) continue;
                if (order.State != RestaurantOrderState.Waiting) continue;

                order.TrySetState(RestaurantOrderState.Preparing);
                return true;
            }

            order = null;
            return false;
        }

        public bool MarkReady(string orderId)
        {
            return orders.TryGetValue(orderId, out var order) &&
                   order.TrySetState(RestaurantOrderState.Ready);
        }

        public bool Serve(string orderId)
        {
            if (!orders.TryGetValue(orderId, out var order) ||
                !order.TrySetState(RestaurantOrderState.Served))
                return false;

            orders.Remove(orderId);
            return true;
        }

        public bool Cancel(string orderId)
        {
            if (!orders.TryGetValue(orderId, out var order) ||
                !order.TrySetState(RestaurantOrderState.Cancelled))
                return false;

            orders.Remove(orderId);
            return true;
        }
    }
}
