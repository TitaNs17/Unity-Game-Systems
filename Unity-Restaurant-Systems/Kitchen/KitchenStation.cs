using System;
using System.Collections;
using UnityEngine;

namespace UnityGameSystems.Restaurant
{
    public sealed class KitchenStation : MonoBehaviour
    {
        [SerializeField] private string stationId;
        [SerializeField, Min(0.1f)] private float speedMultiplier = 1f;

        private Coroutine activeRoutine;

        public string StationId => stationId;
        public bool IsBusy => activeRoutine != null;
        public RestaurantOrder CurrentOrder { get; private set; }

        public event Action<RestaurantOrder> PreparationStarted;
        public event Action<RestaurantOrder> PreparationFinished;

        public bool TryStart(RestaurantOrder order)
        {
            if (order == null || IsBusy || order.State != RestaurantOrderState.Preparing)
                return false;

            CurrentOrder = order;
            activeRoutine = StartCoroutine(Prepare(order));
            return true;
        }

        private IEnumerator Prepare(RestaurantOrder order)
        {
            PreparationStarted?.Invoke(order);

            float duration = 0f;
            foreach (var item in order.Items)
                if (item != null) duration += item.PreparationTime;

            duration /= Mathf.Max(0.1f, speedMultiplier);
            yield return new WaitForSeconds(duration);

            order.TrySetState(RestaurantOrderState.Ready);
            PreparationFinished?.Invoke(order);
            CurrentOrder = null;
            activeRoutine = null;
        }
    }
}
