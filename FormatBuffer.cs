using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public readonly ref struct FormatBuffer
    {
        private readonly Format _format;
        internal FormatBuffer(Format format)
        {
            _format = format;
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
        public void Append(StringRef key)
        {
            _format.Append(ref key);
        }

        public void Append(int number, int @base = 10)
        {
            _format.Append(number, @base);
        }
    }
}