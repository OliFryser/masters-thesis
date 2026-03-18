using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializedDictionary<TKey, TValue>
{
    [SerializeField] private List<KeyValuePair<TKey, TValue>> _values;
    public Dictionary<TKey, TValue> ToDictionary() => _values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    
    public SerializedDictionary(Dictionary<TKey, TValue> dictionary)
    {
        _values = dictionary.Select(kvp => new KeyValuePair<TKey, TValue>
        {
            Key = kvp.Key,
            Value = kvp.Value,
        }).ToList();
    }
    
    [Serializable]
    public class KeyValuePair<TKvpKey, TKvpValue>
    {
        public TKvpKey Key;
        public TKvpValue Value;
    }
}