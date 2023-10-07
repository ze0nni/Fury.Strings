using System;
using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public static unsafe class Helpers
    {
#if WIN32
        public const int PtrSize = sizeof(int);
#else
        public const int PtrSize = sizeof(long);
#endif
        static Helpers()
        {
            if (PtrSize != IntPtr.Size)
            {
                throw new Exception($"Excepted IntPtr.Size={PtrSize} but have {IntPtr.Size}");
            }
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

        public static bool TryParseInt(char* start, int length, out int result)
        {
            if (length == 0)
            {
                result = default;
                return false;
            }

            result = 0;
            var cursor = start + length;
            var m = 1;
            while (cursor-- > start)
            {
                if (*cursor >= '0' && *cursor <= '9')
                {
                    result += m * (*cursor - '0');
                }
                else if (*cursor == '-' && cursor == start)
                {
                    result = -result;
                }
                else
                {
                    result = default;
                    return false;
                }
                m *= 10;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(char* p0, char* p1, int length)
        {
            while (length --> 0) {
                if (*p0++ != *p1++)
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(char* start, int length, string value)
        {
            return IndexOf(start, length, value) != -1;
        }

        public static int IndexOf(char* start, int length, string value)
        {
            var searchLength = value.Length;
            if (searchLength > length)
            {
                return -1;
            }
            if (searchLength == 0)
            {
                return 0;
            }
            var n = length - searchLength + 1;

            var p = start;
            var i = 0;
            fixed (char* valueStart = value)
            {
                while (n-- > 0)
                {
                    if (Compare(p++, valueStart, searchLength))
                    {
                        return i;
                    }
                    i++;
                }
            }
            return -1;
        }

        public static bool StartsWith(char* start, int length, string value)
        {
            if (value.Length > length)
            {
                return false;
            }
            fixed (char* valueStart = value)
            {
                return Compare(start, valueStart, value.Length);
            }
        }

        public static bool EndsWith(char* start, int length, string value)
        {
            if (value.Length > length)
            {
                return false;
            }
            fixed (char* valueStart = value)
            {
                return Compare(start + length - value.Length, valueStart, value.Length);
            }
        }
    }
}
