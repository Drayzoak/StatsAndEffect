using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SAPUnityEditorTools
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : 
        ISerializationCallbackReceiver,
        IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        
        [SerializeField]
        private List<TValue> values = new List<TValue>();

        private Dictionary<TKey, TValue> dictionary = new();

        private bool _initialized;

        public List<TKey> Keys => new List<TKey>(this.dictionary.Keys);
        public List<TValue> Values => new List<TValue>(this.dictionary.Values);
        
        public void EnsureInitialized()
        {
            if (_initialized) return;

            OnAfterDeserialize();
        }

        
        public TValue this[TKey key]
        {
            get
            {
                EnsureInitialized();
                return dictionary[key];
            }
            set
            {
                EnsureInitialized();

                if (dictionary.ContainsKey(key))
                {
                    int index = keys.IndexOf(key);
                    if (index >= 0)
                        values[index] = value;
                }
                else
                {
                    keys.Add(key);
                    values.Add(value);
                }

                dictionary[key] = value;
            }
        }

        
        public bool ContainsKey(TKey key)
        {
            if (key == null)
                return false;
            EnsureInitialized();
            return dictionary.ContainsKey(key);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            EnsureInitialized();

            if (dictionary.ContainsKey(key))
                return false;

            dictionary.Add(key, value);
            keys.Add(key);
            values.Add(value);

            return true;
        }

        public bool Remove(TKey key)
        {
            EnsureInitialized();

            if (!dictionary.TryGetValue(key, out _))
                return false;

            dictionary.Remove(key);

            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                value = default;
                return false;
            }
            EnsureInitialized();
            return dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dictionary.Clear();
            keys.Clear();
            values.Clear();
            _initialized = true;
        }

        
        public void OnBeforeSerialize()
        {
            EnsureInitialized();

            keys.Clear();
            values.Clear();

            foreach (var pair in dictionary)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();

            if (keys.Count != values.Count)
                throw new Exception($"Mismatched key/value count: {keys.Count} keys, {values.Count} values");

            for (int i = 0; i < keys.Count; i++)
            {
                // Prevent duplicate keys crash
                if (!dictionary.ContainsKey(keys[i]))
                {
                    dictionary.Add(keys[i], values[i]);
                }
            }

            _initialized = true;
        }
        
        public int Count
        {
            get
            {
                EnsureInitialized();
                return dictionary.Count;
            }
        }
        public TKey GetKeyAt(int index)
        {
            EnsureInitialized();
            return keys[index];
        }

        public TValue GetValueAt(int index)
        {
            EnsureInitialized();
            return values[index];
        }

        public void SetValueAt(int index, TValue value)
        {
            EnsureInitialized();

            values[index] = value;
            dictionary[keys[index]] = value;
        }

        public int IndexOfKey(TKey key)
        {
            EnsureInitialized();
            return keys.IndexOf(key);
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Enumerable()
        {
            EnsureInitialized();
            return dictionary.AsEnumerable();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            EnsureInitialized();
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IReadOnlyDictionary<TKey, TValue> AsReadOnly()
        {
            EnsureInitialized();
            return dictionary;
        }
    }
}