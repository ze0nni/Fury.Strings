using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public readonly ref struct FormatBuffer
    {
        private readonly ZeroFormat _format;
        internal FormatBuffer(ZeroFormat format)
        {
            _format = format;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Process(string str)
        {
            fixed (char* start = str)
            {
                _format.Process(start, str.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Process(StringKey str)
        {
            fixed (char* start = str)
            {
                _format.Process(start, str.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Process(char* start, int length)
        {
            _format.Process(start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
        {
            _format.Append(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Append(char* src, int length)
        {
            _format.Append(src, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string str)
        {
            _format.Append(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(StringKey key)
        {
            _format.Append(ref key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(StringRef keyRef)
        {
            _format.Append(keyRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(int number, int @base = 10)
        {
            _format.Append(number, @base);
        }
    }
}