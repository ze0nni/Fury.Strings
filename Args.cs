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
        public Args Bool(bool b)
        {
            Append().Bool(b);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Char(char c)
        {
            Append().Char(c);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args CharRepeat(char c, short repeat)
        {
            Append().Char(c, repeat);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Str(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            Append().Str(str);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args StrRange(string str, short start, short length)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (start + length > str.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            Append().StrRange(str, start, length);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Int(int number, byte @base = 10)
        {
            Append().Int(number, @base);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Float(float number, sbyte maxDigitsAfterDecimal = -1)
        {
            Append().Float(number, 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args FloatFixedAfterDecimal(float number, sbyte fixedAfterDecimal = 2)
        {
            Append().FloatFixedAfterDecimal(number, fixedAfterDecimal);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Dobule(double number, sbyte digitsAfterDecimal = -1)
        {
            Append().Float((float)number, 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Obj(object obj)
        {
            Append().Obj(obj);
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
                args.Str(a);
            }
            return args;
        }

        public static implicit operator Args(object[] input)
        {
            var args = new Args();
            foreach (var a in input)
            {
                args.Obj(a);
            }
            return args;
        }
    }
}
