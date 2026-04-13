using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SAPUnityEditorTools
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : ISerializedDictionary , IEnumerable<KeyValuePair<TKey, TValue>>
	{
		[SerializeField]
		private List<TKey> keys = new List<TKey>();

		[SerializeField]
		private List<TValue> values = new List<TValue>();

		private Dictionary<TKey, TValue> thisDictionary = new();

		private bool _initialized;

		public void EnsureInitialized()
		{
			if (_initialized) return;

			OnAfterDeserialize();
			_initialized = true;
		}

		public TValue this[TKey key]
		{
			get
			{
				EnsureInitialized();
				return thisDictionary[key];
			}
			set
			{
				EnsureInitialized();
				thisDictionary[key] = value;
			}
		}

		public bool ContainsKey(TKey key)
		{
			EnsureInitialized();
			return thisDictionary.ContainsKey(key);
		}

		public bool TryAdd(TKey key, TValue value)
		{
			EnsureInitialized();

			if (thisDictionary.ContainsKey(key))
				return false;

			thisDictionary.Add(key, value);
			return true;
		}

		
		public bool Remove(TKey key)
		{
			EnsureInitialized();
			return thisDictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			EnsureInitialized();
			return thisDictionary.TryGetValue(key, out value);
		}

		// save the dictionary to lists
		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();

			foreach (KeyValuePair<TKey, TValue> pair in thisDictionary)
			{
				keys.Add(pair.Key);
				values.Add(pair.Value);
			}
		}

		// load dictionary from lists
		public void OnAfterDeserialize()
		{
			thisDictionary.Clear();
			this._initialized = true;
			if (keys.Count != values.Count)
				throw new Exception($"There are {keys.Count} keys and {values.Count} values after deserialization.");

			for (int i = 0; i < keys.Count; i++)
			{
				if (thisDictionary.ContainsKey(keys[i]))
					continue;

				thisDictionary.Add(keys[i], values[i]);
			}
		}

		public int Count
		{
			get
			{
				this.EnsureInitialized();
				return thisDictionary.Count;
			}
		}
		
		public IEnumerable<KeyValuePair<TKey, TValue>> Enumerable() 
		{
			this.EnsureInitialized();	
			return thisDictionary.AsEnumerable();
		}
		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			EnsureInitialized();
			return thisDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}