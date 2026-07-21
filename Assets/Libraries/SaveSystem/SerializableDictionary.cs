using System.Collections.Generic;
using UnityEngine;

namespace Pospec
{
    /// <summary>
    /// Dictionary, that can be serialized and deserialized to JSON.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            Clear();
            if (keys.Count != values.Count)
            {
                Debug.LogError($"On deserialization of {nameof(SerializableDictionary<TKey, TValue>)} size of {nameof(keys)} doesn't equal size of {nameof(values)}.");
                return;
            }

            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);

            keys.Clear();
            values.Clear();
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }
}
