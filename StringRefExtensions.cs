namespace Fury.Strings
{
    public static class StringRefExtensions
    {
        public static bool TryParseInt(this ref StringRef key, out int result)
        {
            if (key.Length == 0)
            {
                result = default;
                return false;
            }
            unsafe 
            {
                fixed (char* start = key)
                {
                    result = 0;
                    var cursor = start + key.Length;
                    var m = 1;
                    while (cursor-- > start)
                    {
                        if (*cursor >= '0' && *cursor <= '9')
                        {
                            result += m * (*cursor - '0');
                        } else if (*cursor == '-' && cursor == start)
                        {
                            result = -result;
                        } else
                        {
                            result = default;
                            return false;
                        }
                        m *= 10;
                    }
                    return true;
                }
            }
        }
    }
}