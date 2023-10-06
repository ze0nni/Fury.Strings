using System;
using System.Runtime.CompilerServices;

namespace Fury.Strings
{
    public class Format
    {
        private char[] _buffer;
        private int _length;
        
        public Format(int capactity)
        {
            _buffer = new char[capactity];
            _length = 0;
        }

        private string _format;
        private string[] _args;
        private StringDictionary<string> _colorMap;
        public StringDictionary<(string open, string close)> _tagsAlias;
        
        public void Setup(
            string format,
            string[] args = null,
            StringDictionary<string> colorMap = null,
            StringDictionary<(string, string)> tagsAlias = null)
        {
            _format = format;
            _args = args;
            _colorMap = colorMap;
            _tagsAlias = tagsAlias;
        }
        
        public override string ToString()
        {
            _length = 0;
            Process(_format);
            return new string(_buffer, 0, _length);
        }
        
        private void EnsureCapacity()
        {
            var newBuffer = new char[_buffer.Length * 2];
            Array.Copy(_buffer, newBuffer, _buffer.Length);
            _buffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append(char c)
        {
            if (_length == _buffer.Length)
            {
                EnsureCapacity();
            }
            _buffer[_length++] = c;
        }

        private void EnsureCapacity(int newLength)
        {
            var newBuffer = new char[newLength * 2];
            Array.Copy(_buffer, newBuffer, _buffer.Length);
            _buffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Append(char* src, int length)
        {
            var finalLength = _length + length;
            if (finalLength >= _buffer.Length)
            {
                EnsureCapacity(finalLength);
            }
            var n = _length;
            var buffer = _buffer;
            while (length-- > 0)
            {
                buffer[n++] = *src++;
            }
            _length = n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Append(string str)
        {
            fixed (char* cursor = str)
            {
                Append(cursor, str.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Append(ref StringKey key)
        {
            fixed (char* ptr = key)
            {
                Append(ptr, key.Length);
            }
        }

        private unsafe void Process(string format)
        {
            fixed (char* start = format)
            {
                Process(start, format.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Process(char* start, int size)
        {
            var parseHtml = _colorMap != null || _tagsAlias != null;
            var parseArgs = _args != null;

            var cursor = start;
            var end = start + size;
            while (cursor < end)
            {
                if (parseHtml  && * cursor == '<')
                {
                    ParseTag(ref cursor, end);
                } else if (parseArgs  && * cursor == '{')
                {
                    ParseArg(ref cursor, end);
                }
                else
                {
                    Append(*cursor);
                    cursor++;
                }
            }
        }

        private enum ParseTagState
        {
            Open,
            Name,
            Value,
            Close
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void ParseTag(ref char* cursor, char* end)
        {
            var state = ParseTagState.Open;

            var slash = default(bool);
            var name = default(StringKey);
            var value = default(StringKey);

            var revert = cursor;
            
            var pStart = default(char*);
            var pLength = 0;
            
            while (state != ParseTagState.Close)
            {
                cursor++;
                if (cursor >= end)
                {
                    Append(*revert);
                    cursor = revert + 1;
                    return;
                }
                switch (state)
                {
                    case ParseTagState.Open:
                        if (*cursor == '/')
                        {
                            slash = true;
                            pStart = cursor;
                            pLength = 0;
                        }
                        else if (*cursor == '>')
                        {
                            state = ParseTagState.Close;
                            cursor++;
                        }
                        else
                        {
                            state = ParseTagState.Name;
                            pStart = cursor;
                            pLength = 1;
                        }
                        break;

                    case ParseTagState.Name:
                        if (*cursor == '=')
                        {
                            state = ParseTagState.Value;
                            name = new StringKey(pStart, 0, pLength);
                            pStart = cursor+1;
                            pLength = 0;
                        } else if (*cursor == '>')
                        {
                            state = ParseTagState.Close;
                            name = new StringKey(pStart, 0, pLength);
                            cursor++;
                        }
                        else
                        {
                            pLength++;
                        }
                        break;
                    
                    case ParseTagState.Value:
                        if (*cursor == '>')
                        {
                            state = ParseTagState.Close;
                            value = new StringKey(pStart, 0, pLength);
                            cursor++;
                        }
                        else
                        {
                            pLength++;
                        }
                        break;
                }
            }
            
            if (name == "color" && _colorMap != null && _colorMap.TryGetValue(value, out var color))
            {
                Append('<');
                Append(ref name);
                Append('=');
                Append(color);
                Append('>');
            }
            else if (_tagsAlias != null && _tagsAlias.TryGetValue(name, out var alias))
            {
                if (!slash)
                {
                    Append(alias.open);
                }
                else
                {
                    Append(alias.close);
                }
            }
            else
            {
                Append('<');
                if (slash)
                {
                    Append('/');
                }
                Append(ref name);
                if (value.Length > 0)
                {
                    Append('=');
                    Append(ref value);
                }
                Append('>');
            }
        }

        private enum ParseArgState
        {
            Open,
            Body,
            Close,
            Revert
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void ParseArg(ref char* cursor, char* end)
        {
            var state = ParseArgState.Open;

            var revert = cursor;

            var pStart = cursor;
            var pLength = 0;
            var body = default(StringKey);

            while (state != ParseArgState.Close && state != ParseArgState.Revert)
            {
                cursor++;
                if (cursor >= end)
                {
                    state = ParseArgState.Revert;
                }
                switch (state)
                {
                    case ParseArgState.Open:
                        {
                            if (*cursor >= '0' && *cursor <= '9')
                            {
                                state = ParseArgState.Body;
                                pStart = cursor;
                                pLength = 1;
                            } else
                            {
                                state = ParseArgState.Revert;
                            }
                        }
                        break;
                    case ParseArgState.Body:
                        {
                            if (*cursor >= '0' && *cursor <= '9')
                            {
                                pLength++;
                            }
                            else if (*cursor == '}')
                            {
                                state = ParseArgState.Close;
                                body = new StringKey(pStart, 0, pLength);
                                cursor++;
                            }
                            else
                            {
                                state = ParseArgState.Revert;
                            }
                        }
                        break;
                }
            }

            switch (state)
            {
                case ParseArgState.Revert:
                    {
                        Append(*revert);
                        cursor = revert + 1;
                    }
                    break;
                case ParseArgState.Close:
                    {
                        var args = _args;
                        var argsN = args.Length;
                        if (body.TryParseInt(out var n) && n >= 0 && n < argsN)
                        {
                            Append(args[n]);
                        } else
                        {
                            Append("{");
                            Append(ref body);
                            Append("}");
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException(state.ToString());
            }
        }
    }
}