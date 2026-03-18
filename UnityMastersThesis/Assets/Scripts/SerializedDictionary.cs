using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Version_1.Utility
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue>
    {
        [SerializeField] private List<KeyValuePair<TKey, TValue>> _values;
        public Dictionary<TKey, TValue> Build() => _values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        [Serializable]
        public class KeyValuePair<KvpKey, KvpValue>
        {
            public KvpKey Key;
            public KvpValue Value;
        }
    }
}