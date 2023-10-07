using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fury.Strings
{
    public sealed class StringDictionary<TValue> : IDictionary<string, TValue>
    {
        private readonly Dictionary<StringRef, TValue> _origin = new Dictionary<StringRef, TValue>();

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
            _origin.Add(new StringRef(item.Key), item.Value);
        }

        public void Clear()
        {
            _origin.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return _origin.TryGetValue(new StringRef(item.Key), out var value) && value.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            var id = (IDictionary<StringRef, TValue>)_origin;
            return id.Remove(new KeyValuePair<StringRef, TValue>(new StringRef(item.Key), item.Value));
        }

        public int Count => _origin.Count;
        public bool IsReadOnly => false;

        public void Add(string key, TValue value)
        {
            _origin.Add(new StringRef(key), value);
        }

        public bool Remove(string key)
        {
            return _origin.Remove(new StringRef(key));
        }

        public bool ContainsKey(string key) 
            => _origin.ContainsKey(new StringRef(key));

        public bool ContainsKey(string key, int start, int length) =>
            _origin.ContainsKey(new StringRef(key, start, length));

        public bool ContainsKey(char[] chars, int start, int length) =>
            _origin.ContainsKey(new StringRef(chars, start, length));

        public unsafe bool ContainsKey(char* ptr, int start, int length) =>
            _origin.ContainsKey(new StringRef(ptr, start, length));

        public bool ContainsKey(StringRef key) =>
            _origin.ContainsKey(key);

        public bool TryGetValue(string key, out TValue value)
        {
            return _origin.TryGetValue(new StringRef(key), out value);
        }

        public bool TryGetValue(string str, int start, int length, out TValue value)
        {
            return _origin.TryGetValue(new StringRef(str, start, length), out value);
        }

        public bool TryGetValue(char[] chars, int start, int length, out TValue value)
        {
            return _origin.TryGetValue(new StringRef(chars, start, length), out value);
        }

        public unsafe bool TryGetValue(char* ptr, int start, int length, out TValue value)
        {
            return _origin.TryGetValue(new StringRef(ptr, start, length), out value);
        }
        
        public bool TryGetValue(StringRef key, out TValue value)
        {
            return _origin.TryGetValue(key, out value);
        }

        public TValue this[string key]
        {
            get => _origin[new StringRef(key)];
            set => _origin[new StringRef(key)] = value;
        }

        public TValue this[string str, int start, int length] => _origin[new StringRef(str, start, length)];
        public TValue this[char[] chars, int start, int length] => _origin[new StringRef(chars, start, length)];
        public unsafe TValue this[char* ptr, int start, int length] => _origin[new StringRef(ptr, start, length)];
        public TValue this[StringRef key] => _origin[key];

        public ICollection<string> Keys => _origin.Keys.Select(x => x.ToString()).ToArray();
        public ICollection<TValue> Values => _origin.Values;
    }
}