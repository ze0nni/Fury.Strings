namespace Fury.Strings
{
    public static class FormatExtensions
    {
        public static unsafe void Append(this Format format, int number)
        {
            if (number  < 0)
            {
                format.Append('-');
                number = -number;
            }
            var numDigits = 0;
            for (var n = number; n > 0; n /= 10) numDigits++;
            var digits = stackalloc char[numDigits];

            var pos = numDigits - 1;
            while (number > 0)
            {
                var digit = number % 10;
                digits[pos--] = (char)('0' + digit);
                number = number / 10;
            }
            format.Append(digits, numDigits);
        }
    }
}
