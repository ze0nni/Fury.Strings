using System;
using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public sealed class Args
    {
        private readonly Action _onChanged;
        private Arg[] _items = new Arg[8];
        private object[] _objItems = new object[8];
        private int _length;
        public int Length => _length;

        public Args(Action onChanged = null)
        {
            _onChanged = onChanged;
        }

        internal ref Arg this[int index] => ref _items[index];

        public Args Clear()
        {
            if (_length == 0)
            {
                return this;
            }
            _length = 0;
            _onChanged?.Invoke();
            return this;
        }

        private ref Arg Append()
        {
            if (_length == _items.Length)
            {
                var newItems = new Arg[_items.Length * 2];
                Array.Copy(_items, newItems, _items.Length);
                _items = newItems;
            }
            return ref _items[_length++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(bool b)
        {
            Append().Set(b);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(char c)
        {
            Append().Set(c);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            Append().Set(str);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(string str, short start, short length)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (start + length > str.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            Append().Set(str, start, length);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(int number, byte @base = 10)
        {
            Append().Set(number, @base);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(float number, sbyte digitsAfterDecimal = -1)
        {
            Append().Set(number, 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(double number, sbyte digitsAfterDecimal = -1)
        {
            Append().Set((float)number, 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Add(object obj)
        {
            Append().Set(obj);
            return this;
        }

        public object[] ToObjectsArray()
        {
            if (_length > _objItems.Length)
            {
                _objItems = new object[_length * 2];
            }

            for (var i = 0; i < _length; i++)
            {
                ref var arg = ref _items[i];
                _objItems[i] = arg.ToObject();
            }

            return _objItems;
        }

        public static implicit operator Args(string[] input)
        {
            var args = new Args();
            foreach (var a in input)
            {
                args.Add(a);
            }
            return args;
        }

        public static implicit operator Args(object[] input)
        {
            var args = new Args();
            foreach (var a in input)
            {
                args.Add(a);
            }
            return args;
        }
    }
}
