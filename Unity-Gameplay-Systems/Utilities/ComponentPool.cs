using System.Collections.Generic;
using UnityEngine;

namespace UnityGameSystems.Utilities
{
    public sealed class ComponentPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Stack<T> inactive = new();
        private readonly HashSet<T> active = new();

        public int ActiveCount => active.Count;
        public int InactiveCount => inactive.Count;

        public ComponentPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (var i = 0; i < initialSize; i++)
                inactive.Push(CreateInstance());
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            var instance = inactive.Count > 0 ? inactive.Pop() : CreateInstance();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            active.Add(instance);
            return instance;
        }

        public bool Release(T instance)
        {
            if (instance == null || !active.Remove(instance))
                return false;

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(parent, false);
            inactive.Push(instance);
            return true;
        }

        private T CreateInstance()
        {
            var instance = Object.Instantiate(prefab, parent);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
