using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fury.Strings
{
    public class StringDictionary<TValue> : IDictionary<string, TValue>
    {
        private readonly Dictionary<StringKey, TValue> _origin = new Dictionary<StringKey, TValue>();

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            foreach (var e in _origin)
            {
                yield return new KeyValuePair<string, TValue>(e.Key.ToString(), e.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            _origin.Add(new StringKey(item.Key), item.Value);
        }

        public void Clear()
        {
            _origin.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return _origin.TryGetValue(new StringKey(item.Key), out var value) && value.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            var id = (IDictionary<StringKey, TValue>)_origin;
            return id.Remove(new KeyValuePair<StringKey, TValue>(new StringKey(item.Key), item.Value));
        }

        public int Count => _origin.Count;
        public bool IsReadOnly => false;

        public void Add(string key, TValue value)
        {
            _origin.Add(new StringKey(key), value);
        }

        public bool Remove(string key)
        {
            return _origin.Remove(new StringKey(key));
        }

        public bool ContainsKey(string key) 
            => _origin.ContainsKey(new StringKey(key));

        public bool ContainsKey(string key, int start, int length) =>
            _origin.ContainsKey(new StringKey(key, start, length));

        public bool ContainsKey(char[] chars, int start, int length) =>
            _origin.ContainsKey(new StringKey(chars, start, length));

        public unsafe bool ContainsKey(char* ptr, int start, int length) =>
            _origin.ContainsKey(new StringKey(ptr, start, length));

        public bool ContainsKey(StringKey key) =>
            _origin.ContainsKey(key);

        public bool TryGetValue(string key, out TValue value)
        {
            return _origin.TryGetValue(new StringKey(key), out value);
        }

        public bool TryGetValue(string str, int start, int length, out TValue value)
        {
            return _origin.TryGetValue(new StringKey(str, start, length), out value);
        }

        public bool TryGetValue(char[] chars, int start, int length, out TValue value)
        {
            return _origin.TryGetValue(new StringKey(chars, start, length), out value);
        }

        public unsafe bool TryGetValue(char* ptr, int start, int length, out TValue value)
        {
            return _origin.TryGetValue(new StringKey(ptr, start, length), out value);
        }
        
        public bool TryGetValue(StringKey key, out TValue value)
        {
            return _origin.TryGetValue(key, out value);
        }

        public TValue this[string key]
        {
            get => _origin[new StringKey(key)];
            set => _origin[new StringKey(key)] = value;
        }

        public TValue this[string str, int start, int length] => _origin[new StringKey(str, start, length)];
        public TValue this[char[] chars, int start, int length] => _origin[new StringKey(chars, start, length)];
        public unsafe TValue this[char* ptr, int start, int length] => _origin[new StringKey(ptr, start, length)];
        public TValue this[StringKey key] => _origin[key];

        public ICollection<string> Keys => _origin.Keys.Select(x => x.ToString()).ToArray();
        public ICollection<TValue> Values => _origin.Values;
    }
}