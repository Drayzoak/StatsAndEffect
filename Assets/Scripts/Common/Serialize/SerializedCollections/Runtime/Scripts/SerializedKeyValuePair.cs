using Common.Serialize;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    [System.Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        [SerializeReference]
        public TValue Value;
        public SerializedType ValueType;
        public string Data;
        public bool IsAbstract;
        public SerializedKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            IsAbstract = typeof(TValue).IsAbstract;
            if (IsAbstract)
            {
                ValueType = value.GetType();
                Data = JsonUtility.ToJson(value);
            }
            else
            {
                ValueType = null;
                Data = null;
            }
        }
    }
}
