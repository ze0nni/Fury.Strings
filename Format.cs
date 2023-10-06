using System;
using System.Runtime.CompilerServices;
using UnityEngine;

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
        private StringDictionary<string> _colorMap;
        public StringDictionary<(string open, string close)> _tagsAlias;
        
        public void Setup(
            string format,
            StringDictionary<string> colorMap = null,
            StringDictionary<(string, string)> tagsAlias = null)
        {
            _length = 0;
            _format = format;
            _colorMap = colorMap;
            _tagsAlias = tagsAlias;
        }
        
        public override string ToString()
        {
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
                EnsureCapacity();
            _buffer[_length++] = c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Append(char* cursor, int length)
        {
            while (length-- > 0)
            {
                Append(*cursor);
                cursor++;
            }
        }

        private unsafe void Append(string str)
        {
            fixed (char* cursor = str)
            {
                Append(cursor, str.Length);
            }
        }

        private unsafe void Append(StringKey key)
        {
            var cursor = key.Pin(out var pinned, out var handle);
            Append(cursor, key.Length);
            if (pinned) handle.Free();
        }

        private unsafe void Process(string format)
        {
            fixed (char* start = format)
            {
                Process(start, format.Length);
            }
        }

        private unsafe void Process(char* start, int size)
        {
            var cursor = start;
            var end = start + size;
            while (cursor < end)
            {
                if (*cursor == '<')
                {
                    ParseTag(ref cursor, end);
                } else if (*cursor == '{')
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
                Append(name);
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
                Append(name);
                if (value.Length > 0)
                {
                    Append('=');
                    Append(value);
                }
                Append('>');
            }
        }

        private unsafe void ParseArg(ref char* cursor, char* end)
        {
            
        }
    }
}