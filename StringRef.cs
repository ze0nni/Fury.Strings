using System;
using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public readonly unsafe ref struct StringRef
    {
        internal readonly char* _ptr;
        public readonly int Length;
        internal StringRef(char* ptr, int length)
        {
            _ptr = ptr;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StringKey ToKey()
        {
            return new StringKey(_ptr, 0, Length);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case StringKey key: return this.ToKey() == key;
                case string str: return this == str;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return StringKey.GetHashCode(_ptr, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(StringRef ref0, StringRef ref1)
        {
            var n = ref0.Length;
            if (n != ref1.Length)
            {
                return false;
            }
            if (n == 0)
            {
                return true;
            }
            var p0 = ref0._ptr;
            var p1 = ref1._ptr;
            while (n --> 0)
            {
                if (*p0++ != *p1++)
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(StringRef ref0, StringRef ref1)
        {
            return !(ref0 == ref1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator==(StringRef strRef, string str)
        {
            var n = strRef.Length;
            if (n != str.Length)
            {
                return false;
            }
            if (n == 0)
            {
                return true;
            }
            var p0 = strRef._ptr;
            fixed (char* strPtr = str)
            {
                var p1 = strPtr;
                while (n --> 0) {
                    if (*p0++ != *p1++)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator !=(StringRef strRef, string str)
        {
            return !(strRef == str);
        }
    }
}
