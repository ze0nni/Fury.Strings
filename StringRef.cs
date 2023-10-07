using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Fury.Strings
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly unsafe struct StringRef : IEquatable<StringRef>, IEquatable<string>
    {
#if WIN32
        const int PtrSize = sizeof(int);
#else
        const int PtrSize = sizeof(long);
#endif
        static StringRef() {
            if (PtrSize != IntPtr.Size)
            {
                throw new Exception($"Excepted IntPtr.Size={PtrSize} but have {IntPtr.Size}");
            }
        }

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
        [FieldOffset(PtrSize)] private readonly RefType _type;
        [FieldOffset(PtrSize + sizeof(byte))] private readonly int _start;
        [FieldOffset(PtrSize + sizeof(byte) + sizeof(int))] public readonly int Length;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringRef(string str)
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
        public StringRef(string str, int start, int length)
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
        public StringRef(char[] chars, int start, int length)
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
        public StringRef(char* ptr, int start, int length)
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
                return ref UnsafeUtility.AsRef<char>(IntPtr.Zero.ToPointer());
            }
            switch (_type)
            {
                case RefType.Str:
                    fixed (char* s = _str) return ref UnsafeUtility.AsRef<char>(s + _start);
                case RefType.Chars:
                    fixed (char* s = _chars) return ref UnsafeUtility.AsRef<char>(s + _start);
                case RefType.Ptr:
                    return ref UnsafeUtility.AsRef<char>(_ptr + _start);
                default:
                    throw new ArgumentOutOfRangeException(_type.ToString(), nameof(_type));
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(StringRef other)
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
            return Equals(new StringRef(other));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is StringRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            fixed (char* ptr = this)
            {
                return GetHashCode(ptr, Length);
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
        public static bool operator ==(StringRef a, StringRef b)
        {
            return a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StringRef a, StringRef b)
        {
            return !a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(StringRef a, string b)
        {
            return a.Equals(b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StringRef a, string b)
        {
            return !a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StringRef(string str)
        {
            return new StringRef(str);
        }

        public static int GetHashCode(char* s, int length)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (var i = 0; i < length; i++)
            {
                hash1 = ((hash1 << 5) + hash1) ^ *s;
                if (*s == 0)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ *s;
                s++;
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}