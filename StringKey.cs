using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Codice.CM.SemanticMerge.Gui;

namespace Fury.Strings
{
    public readonly unsafe struct StringKey : IEquatable<StringKey>, IEquatable<string>
    {
        private readonly string _str;
        private readonly char[] _chars;
        private readonly char* _ptr;
        private readonly int _start;
        public readonly int Length;
        
        public StringKey(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            _str = str;
            _chars = null;
            _ptr = null;
            _start = 0;
            Length = _str.Length;
        }

        public StringKey(string str, int start, int length)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (start < 0 || start + length >= str.Length)
                throw new ArgumentOutOfRangeException();
            _str = str;
            _chars = null;
            _ptr = null;
            _start = start;
            Length = length;
        }

        public StringKey(char[] chars, int start, int length)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (start < 0 || start + length >= chars.Length)
                throw new ArgumentOutOfRangeException();
            _str = null;
            _chars = chars;
            _ptr = null;
            _start = start;
            Length = length;
        }

        public StringKey(char* ptr, int start, int length)
        {
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            _str = null;
            _chars = null;
            _ptr = ptr;
            _start = start;
            Length = length;
        }

        public char* Pin(out bool pinned, out GCHandle handle)
        {
            if (Length == 0)
            {
                pinned = false;
                handle = default;
                return null;
            }
            
            char* ptr;
            if (_str != null)
            {
                pinned = true;
                handle = GCHandle.Alloc(_str, GCHandleType.Pinned);
                ptr = (char*)handle.AddrOfPinnedObject().ToPointer();
            }
            else if (_chars != null)
            {
                pinned = true;
                handle = GCHandle.Alloc(_chars, GCHandleType.Pinned);
                ptr = (char*)handle.AddrOfPinnedObject().ToPointer();
            }
            else if (_ptr != null)
            {
                pinned = false;
                handle = default;
                ptr = _ptr;
            }
            else
            {
                throw new NotImplementedException();
            }

            return ptr + _start;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(StringKey other)
        {
            if (this.Length != other.Length)
            {
                return false;
            }

            if (this.Length == 0 && other.Length == 0)
            {
                return true;
            }

            var c0 = Pin(out var p0, out var h0);
            var c1 = Pin(out var p1, out var h1);

            try
            {
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
            finally
            {
                if (p0) h0.Free();
                if (p1) h1.Free();
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
            var ptr = Pin(out var pinned, out var handle);
            var hash = GetHashCode(ptr, Length);
            if (pinned) handle.Free();
            return hash;
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

            var ptr = Pin(out var pinned, out var handle);
            var result = new string(ptr, 0, Length);
            if (pinned) handle.Free();
            return result;
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