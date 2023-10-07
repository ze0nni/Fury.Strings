using System;
using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public static unsafe class StringRefExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseInt(this ref StringRef strRef, out int result)
        {
            return Helpers.TryParseInt(strRef._ptr, strRef.Length, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringRef Substring(this ref StringRef strRef, int startIndex)
        {
            return strRef.Substring(startIndex, strRef.Length - startIndex);
        }

        public static StringRef Substring(this ref StringRef strRef, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= strRef.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("Must be positive", nameof(length));
            }
            if (startIndex + length > strRef.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            var c = strRef._ptr + startIndex;
            return new StringRef(c, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this ref StringRef strRef, string value)
        {
            return Helpers.Contains(strRef._ptr, strRef.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(this ref StringRef strRef, string value)
        {
            return Helpers.IndexOf(strRef._ptr, strRef.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this ref StringRef strRef, string value)
        {
            return Helpers.StartsWith(strRef._ptr, strRef.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWith(this ref StringRef strRef, string value)
        {
            return Helpers.EndsWith(strRef._ptr, strRef.Length, value);
        }
    }
}
