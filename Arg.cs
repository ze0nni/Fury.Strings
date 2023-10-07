using System;
using System.Runtime.InteropServices;

namespace Fury.Strings
{
    public enum ArgType: byte
    {
        Null,
        Bool,
        Char,
        String,
        StringRange,
        Int,
        Float,
        Object,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Arg
    {
        [FieldOffset(0)] private string _str;
        [FieldOffset(0)] private object _obj;

        [FieldOffset(Helpers.PtrSize)] private bool _bool;
        [FieldOffset(Helpers.PtrSize)] private char _char;
        [FieldOffset(Helpers.PtrSize)] private int _int;
        [FieldOffset(Helpers.PtrSize)] private float _float;

        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 0)] byte _b0;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 1)] byte _b1;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 2)] byte _b2;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 3)] byte _b3;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 0)] sbyte _sb0;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 1)] sbyte _sb1;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 2)] sbyte _sb2;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 3)] sbyte _sb3;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 0)] short _s0;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 2)] short _s1;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 0)] ushort _us0;
        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(byte) * 2)] ushort _us2;

        [FieldOffset(Helpers.PtrSize + sizeof(int) + sizeof(int))] private ArgType _type;

        internal void Set(bool b)
        {
            _type = ArgType.Bool;
            _bool = b;
        }

        internal void Set(char c)
        {
            _type = ArgType.Char;
            _char = c;
        }

        internal void Set(string str)
        {
            _type = ArgType.String;
            _str = str;
        }

        internal void Set(string str, short start, short length)
        {
            _type = ArgType.StringRange;
            _str = str;
            _s0 = start;
            _s1 = length;
        }

        internal void Set(int number, byte numberBase = 10)
        {
            _type = ArgType.Int;
            _int = number;
            _b0 = numberBase;
        }

        internal void Set(float number, sbyte digitsAfterDecimal = -1)
        {
            _type = ArgType.Float;
            _float = number;
            _sb0 = digitsAfterDecimal;
        }

        internal void Set(object obj)
        {
            _type = ArgType.Object;
            _obj = obj;
        }

        internal unsafe bool Append(ZeroFormat format)
        {
            switch (_type)
            {
                case ArgType.Null:
                    return false;
                case ArgType.Bool:
                    format.Append(_bool ? "true" : "false");
                    return true;
                case ArgType.Char:
                    format.Append(_char);
                    return true;
                case ArgType.String:
                    format.Append(_str);
                    return true;
                case ArgType.StringRange:
                    {
                        fixed (char* strStart = _str)
                        {
                            format.Append(strStart + _s0, _s1);
                        }
                    }
                    return true;
                case ArgType.Int:
                    format.Append(_int, _b0);
                    return true;
                case ArgType.Float:
                    format.Append(_float, _sb0);
                    return true;
                case ArgType.Object:
                    format.Append(_obj == null ? "null" :_obj.ToString());
                    return true;
                default:
                    throw new NotImplementedException(_type.ToString());
            }
        }

        internal object ToObject()
        {
            switch (_type)
            {
                case ArgType.Null:
                    return null;
                case ArgType.Bool:
                    return _bool ? "true" : "false";
                case ArgType.Char:
                    return _char.ToString();
                case ArgType.String:
                    return _str;
                case ArgType.StringRange:
                    return _str.Substring(_s0, _s1);
                case ArgType.Int:
                    return _int.ToString();
                case ArgType.Float:
                    return _float.ToString();
                case ArgType.Object:
                    return _obj;
                default:
                    throw new NotImplementedException(_type.ToString());
            }
        }
    }
}