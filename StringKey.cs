using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Fury.Strings
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly unsafe struct StringKey : IEquatable<StringKey>, IEquatable<string>
    {
        private enum RefType : byte
        {
            Null,
            Str,
            Chars,
            Ptr
        }

        [FieldOffset(0)] private readonly string _str;
        [FieldOffset(0)] private readonly char[] _chars;
        [FieldOffset(0)] private readonly char* _ptr;
        [FieldOffset(Helpers.PtrSize)] private readonly RefType _type;
        [FieldOffset(Helpers.PtrSize + sizeof(byte))] private readonly int _start;
        [FieldOffset(Helpers.PtrSize + sizeof(byte) + sizeof(int))] public readonly int Length;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringKey(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            _chars = null;
            _ptr = null;

            _type = RefType.Str;
            _str = str;
            _start = 0;
            Length = str.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringKey(string str, int start, int length)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (start < 0 || start + length >= str.Length)
                throw new ArgumentOutOfRangeException();
            _chars = null;
            _ptr = null;

            _type = RefType.Str;
            _str = str;
            _start = start;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringKey(char[] chars, int start, int length)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (start < 0 || start + length >= chars.Length)
                throw new ArgumentOutOfRangeException();
            _str = null;
            _ptr = null;

            _type = RefType.Chars;
            _chars = chars;
            _start = start;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StringKey(char* ptr, int start, int length)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            _str = null;
            _chars = null;

            _type = RefType.Ptr;
            _ptr = ptr;
            _start = start;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ref readonly char GetPinnableReference()
        {
            if (Length == 0)
            {
                return ref Helpers.AsRef<char>(IntPtr.Zero.ToPointer());
            }
            switch (_type)
            {
                case RefType.Str:
                    fixed (char* s = _str) return ref Helpers.AsRef<char>(s + _start);
                case RefType.Chars:
                    fixed (char* s = _chars) return ref Helpers.AsRef<char>(s + _start);
                case RefType.Ptr:
                    return ref Helpers.AsRef<char>(_ptr + _start);
                default:
                    throw new ArgumentOutOfRangeException(_type.ToString(), nameof(_type));
            }
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(StringKey other)
        {
            if (this.Length != other.Length)
            {
                return false;
            }

            if (Length == 0)
            {
                return true;
            }

            fixed(char* p0 = this, p1 = other)
            {
                var c0 = p0;
                var c1 = p1;
                var n = Length;
                while (n-- > 0)
                {
                    if (*c0 != *c1)
                    {
                        return false;
                    }

                    c0++;
                    c1++;
                }
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string other)
        {
            return Equals(new StringKey(other));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is StringKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            fixed (char* ptr = this)
            {
                return Helpers.GetHashCode(ptr, Length);
            }
        }

        public override string ToString()
        {
            if (Length == 0)
            {
                return string.Empty;
            }
            
            if (_str != null && _start == 0 && Length == _str.Length)
            {
                return _str;
            }

            fixed (char* ptr = this)
            {
                return new string(ptr, 0, Length);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(StringKey a, StringKey b)
        {
            return a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StringKey a, StringKey b)
        {
            return !a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(StringKey a, string b)
        {
            return a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StringKey a, string b)
        {
            return !a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StringKey(string str)
        {
            return new StringKey(str);
        }
    }
}