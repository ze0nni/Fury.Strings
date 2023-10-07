using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public static unsafe class StringKeyExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseInt(this ref StringKey key, out int result)
        {
            fixed (char* start = key)
            {
                return Helpers.TryParseInt(start, key.Length, out result);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this ref StringKey key, string value)
        {
            fixed (char* start = key)
            {
                return Helpers.Contains(start, key.Length, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(this ref StringKey key, string value)
        {
            fixed (char* start = key)
            {
                return Helpers.IndexOf(start, key.Length, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith(this ref StringKey key, string value)
        {
            fixed (char* start = key)
            {
                return Helpers.StartsWith(start, key.Length, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWith(this ref StringKey key, string value)
        {
            fixed (char* start = key)
            {
                return Helpers.EndsWith(start, key.Length, value);
            }
        }
    }
}