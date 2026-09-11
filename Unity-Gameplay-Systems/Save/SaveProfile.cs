using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameSystems.Save
{
    [Serializable]
    public sealed class SaveValue
    {
        public string key;
        public string value;
    }

    [Serializable]
    public sealed class SaveProfile
    {
        public int version = 2;
        public string sceneName;
        public Vector3 playerPosition;
        public Vector3 playerEulerAngles;
        public float playTimeSeconds;
        public List<SaveValue> values = new List<SaveValue>();

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].key != key) continue;
                values[i].value = value;
                return;
            }

            values.Add(new SaveValue { key = key, value = value });
        }

        public bool TryGet(string key, out string value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].key != key) continue;
                value = values[i].value;
                return true;
            }

            value = null;
            return false;
        }
    }
}
