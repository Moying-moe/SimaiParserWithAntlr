using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace SimaiParserWithAntlr.Utils
{

    public class TwoWayDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull where TValue : notnull
    {
        private readonly Dictionary<TKey, TValue> _keyToValueDict = new();
        private readonly Dictionary<TValue, TKey> _valueToKeyDict = new();

        public int Count => _keyToValueDict.Count;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _keyToValueDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue GetValue(TKey key)
        {
            if (_keyToValueDict.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"{key}");
        }

        public TValue GetValueOrDefault(TKey key, TValue defaultValue)
        {
            return _keyToValueDict.GetValueOrDefault(key, defaultValue);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue result)
        {
            return _keyToValueDict.TryGetValue(key, out result);
        }

        public TKey GetKey(TValue value)
        {
            if (_valueToKeyDict.TryGetValue(value, out var key))
            {
                return key;
            }

            throw new KeyNotFoundException($"{value}");
        }

        public TKey GetKeyOrDefault(TValue value, TKey defaultKey)
        {
            return _valueToKeyDict.GetValueOrDefault(value, defaultKey);
        }

        public bool TryGetKey(TValue value, [MaybeNullWhen(false)] out TKey result)
        {
            return _valueToKeyDict.TryGetValue(value, out result);
        }

        public void Add(TKey key, TValue value)
        {
            try
            {
                RemoveKey(key);
            }
            catch (KeyNotFoundException)
            {
            }

            try
            {
                RemoveValue(value);
            }
            catch (KeyNotFoundException)
            {
            }

            _keyToValueDict.Add(key, value);
            _valueToKeyDict.Add(value, key);
        }

        public bool RemoveKey(TKey key)
        {
            if (!_keyToValueDict.TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException($"{key}");
            }

            return _keyToValueDict.Remove(key) && _valueToKeyDict.Remove(value);
        }

        public bool RemoveValue(TValue value)
        {
            if (!_valueToKeyDict.TryGetValue(value, out var key))
            {
                throw new KeyNotFoundException($"{value}");
            }

            return _keyToValueDict.Remove(key) && _valueToKeyDict.Remove(value);
        }
    }
}