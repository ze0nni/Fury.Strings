using System;
using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public sealed class Args
    {
        private readonly Action _onChanged;
        private bool _onChangedTriggered;
        private Arg[] _items = new Arg[8];
        private object[] _objItems = new object[8];
        private int _length;
        public int Length => _length;

        public Args(out Action clear, Action onChanged = null)
        {
            clear = Clear;
            _onChanged = onChanged;
        }

        internal ref Arg this[int index] => ref _items[index];

        internal void Clear()
        {
            if (_length == 0)
            {
                return;
            }
            _length = 0;
            _onChangedTriggered = false;
        }

        void NotifyChanged()
        {
            if (_onChangedTriggered)
            {
                return;
            }
            _onChangedTriggered = true;
            _onChanged?.Invoke();
        }

        private ref Arg Append()
        {
            if (_length == _items.Length)
            {
                var newItems = new Arg[_items.Length * 2];
                Array.Copy(_items, newItems, _items.Length);
                _items = newItems;
                NotifyChanged();
            }
            return ref _items[_length++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Bool(bool b)
        {
            if (Append().Bool(b))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Char(char c)
        {
            if (Append().Char(c))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args CharRepeat(char c, short repeat)
        {
            if (Append().Char(c, repeat))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Str(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (Append().Str(str))
            {
                NotifyChanged();
            }
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
            if (Append().StrRange(str, start, length))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Int(int number, byte @base = 10)
        {
            if (Append().Int(number, @base))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Float(float number, sbyte maxDigitsAfterDecimal = 2)
        {
            if (Append().Float(number, maxDigitsAfterDecimal))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args FloatFixed(float number, sbyte fixedAfterDecimal = 2)
        {
            if (Append().FloatFixed(number, fixedAfterDecimal))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Dobule(double number, sbyte maxDigitsAfterDecimal = 2)
        {
            if (Append().Float((float)number, maxDigitsAfterDecimal))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args DobuleFixed(double number, sbyte fixedAfterDecimal = 2)
        {
            if (Append().FloatFixed((float)number, fixedAfterDecimal))
            {
                NotifyChanged();
            }
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Args Obj(object obj)
        {
            if (Append().Obj(obj))
            {
                NotifyChanged();
            }
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
            var args = new Args(out _);
            foreach (var a in input)
            {
                args.Str(a);
            }
            return args;
        }

        public static implicit operator Args(object[] input)
        {
            var args = new Args(out _);
            foreach (var a in input)
            {
                args.Obj(a);
            }
            return args;
        }
    }
}
