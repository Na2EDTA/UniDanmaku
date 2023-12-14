using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Danmaku.SerializeExtension
{
    public class Dictionary { }

    [Serializable]
    public class Dictionary<TKey, TValue> : Dictionary, ISerializationCallbackReceiver, IDictionary<TKey, TValue>
    {
        [SerializeField]
        private List<KeyValuePair> list = new();

        [Serializable]
        public struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public KeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private Lazy<System.Collections.Generic.Dictionary<TKey, int>> _keyPositions;
        private System.Collections.Generic.Dictionary<TKey, int> KeyPositions => _keyPositions.Value;

        public Dictionary()
        {
            _keyPositions = new Lazy<System.Collections.Generic.Dictionary<TKey, int>>(MakeKeyPositions);
        }

        private System.Collections.Generic.Dictionary<TKey, int> MakeKeyPositions()
        {
            var dict = new System.Collections.Generic.Dictionary<TKey, int>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                dict[list[i].Key] = i;
            }
            return dict;
        }

        public void OnAfterDeserialize()
        {
            _keyPositions = new Lazy<System.Collections.Generic.Dictionary<TKey, int>>(MakeKeyPositions);
        }

        public void OnBeforeSerialize() { }

        #region IDictionary<TKey, TValue>

        public TValue this[TKey key] 
        { 
            get => list[KeyPositions[key]].Value;
            set
            {
                var pair = new KeyValuePair(key, value);
                if (KeyPositions.ContainsKey(key))
                {
                    list[KeyPositions[key]] = pair;
                }
                else
                {
                    KeyPositions[key] = list.Count;
                    list.Add(pair);
                }
            }
        }

        public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();

        public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

        public void Add(TKey key, TValue value)
        {
            if (KeyPositions.ContainsKey(key))
                Debug.LogWarning("An element with the same key already exists in the dictionary.");
            else
            {
                KeyPositions[key] = list.Count;
                list.Add(new KeyValuePair(key, value));
            }
        }

        public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

        public bool Remove(TKey key)
        {
            if (KeyPositions.TryGetValue(key, out var index))
            {
                KeyPositions.Remove(key);

                list.RemoveAt(index);
                for (var i = index; i < list.Count; i++)
                    KeyPositions[list[i].Key] = i;

                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (KeyPositions.TryGetValue(key, out var index))
            {
                value = list[index].Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        #endregion

        #region ICollection <KeyValuePair<TKey, TValue>>

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear() => list.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => KeyPositions.ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var numKeys = list.Count;
            if (array.Length - arrayIndex < numKeys)
                throw new ArgumentException("arrayIndex");
            for (var i = 0; i < numKeys; i++, arrayIndex++)
            {
                var entry = list[i];
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        #endregion

        #region IEnumerable <KeyValuePair<TKey, TValue>>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return list.Select(ToKeyValuePair).GetEnumerator();
        }

        static KeyValuePair<TKey, TValue> ToKeyValuePair(KeyValuePair danmakuPair)
        {
            return new KeyValuePair<TKey, TValue>(danmakuPair.Key, danmakuPair.Value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}

