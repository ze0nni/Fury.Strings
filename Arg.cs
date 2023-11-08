using System;
using System.Runtime.InteropServices;

namespace Fury.Strings
{
    public enum ArgType: byte
    {
        Null,
        Bool,
        Char,
        CharRepeat,
        String,
        StringRange,
        Int,
        Float,
        FloatFixedAfterDecimal,
        Timer,
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

        internal bool Bool(bool b)
        {
            if (_type == ArgType.Bool 
                && _bool == b)
            {
                return false;
            }
            _type = ArgType.Bool;
            _bool = b;
            return true;
        }

        internal bool Char(char c)
        {
            if(_type == ArgType.Char 
                && _char == c)
            {
                return false;
            }
            _type = ArgType.Char;
            _char = c;
            return true;
        }

        internal bool Char(char c, short repeats)
        {
            if(_type == ArgType.CharRepeat
                && _char == c
                && _s0 == repeats)
            {
                return false;
            }
            _type = ArgType.CharRepeat;
            _char = c;
            _s0 = repeats;
            return true;
        }

        internal bool Str(string str)
        {
            if(_type == ArgType.String
                && _str == str)
            {
                return false;
            }
            _type = ArgType.String;
            _str = str;
            return true;
        }

        internal bool StrRange(string str, short start, short length)
        {
            if(_type == ArgType.StringRange
                && _str == str
                && _s0 == start
                & _s1 == length)
            {
                return false;
            }
            _type = ArgType.StringRange;
            _str = str;
            _s0 = start;
            _s1 = length;
            return true;
        }

        internal bool Int(int number, byte numberBase = 10)
        {
            if(_type == ArgType.Int
                && _int == number
                && _b0 == numberBase)
            {
                return false;
            }
            _type = ArgType.Int;
            _int = number;
            _b0 = numberBase;
            return true;
        }

        internal bool Float(float number, sbyte maxDigitsAfterDecimal = -1)
        {
            if(_type == ArgType.Float
                && _float == number
                && _sb0 == maxDigitsAfterDecimal)
            {
                return false;
            }
            _type = ArgType.Float;
            _float = number;
            _sb0 = maxDigitsAfterDecimal;
            return true;
        }

        internal bool FloatFixed(float number, sbyte fixedAfterDecimal= 2)
        {
            if(_type == ArgType.FloatFixedAfterDecimal
                && _float == number
                && _sb0 == fixedAfterDecimal)
            {
                return false;
            }
            _type = ArgType.FloatFixedAfterDecimal;
            _float = number;
            _sb0 = fixedAfterDecimal;
            return true;
        }

        internal bool Timer(int seconds, TimerFormatFlags flags)
        {
            if (_type == ArgType.Timer
                && _int == seconds
                && _b0 == (byte)flags)
            {
                return false;
            }
            _type = ArgType.Timer;
            _int = seconds;
            _b0 = (byte)flags;
            return true;
        }

        internal bool Obj(object obj)
        {
            if(_type == ArgType.Object
                && object.Equals(_obj, obj))
            {
                return false;
            }
            _type = ArgType.Object;
            _obj = obj;
            return true;
        }

        internal unsafe bool Write(ZeroFormat format)
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
                case ArgType.CharRepeat:
                    for (var i = 0; i < _s0; i++) {
                        format.Append(_char);
                    }
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
                    format.Append(_float, ZeroFormatExtensions.AfterDecimalFormat.MaxNonZero, _sb0);
                    return true;
                case ArgType.FloatFixedAfterDecimal:
                    format.Append(_float, ZeroFormatExtensions.AfterDecimalFormat.Fixed, _sb0);
                    return true;
                case ArgType.Timer:
                    format.AppendTimer(_int, (TimerFormatFlags)_b0);
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
                case ArgType.CharRepeat:
                    return new string(_char, _s0);
                case ArgType.String:
                    return _str;
                case ArgType.StringRange:
                    return _str.Substring(_s0, _s1);
                case ArgType.Int:
                    return _int.ToString();
                case ArgType.Float:
                    return _float.ToString();
                case ArgType.FloatFixedAfterDecimal:
                    return _float.ToString();
                case ArgType.Object:
                    return _obj;
                default:
                    throw new NotImplementedException(_type.ToString());
            }
        }
    }
}