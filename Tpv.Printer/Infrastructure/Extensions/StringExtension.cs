using System;
using System.Linq;

namespace Tpv.Printer.Infrastructure.Extensions
{
    public static class StringExtension
    {
        public static string ToMax(this string value, int max, bool isNull = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return isNull ? null : string.Empty;

            if (value.Length > max)
                return value.Substring(0, max);

            return value;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string ReplaceWildCardByEnvironmentVariables(this string line, char wildCard)
        {
            var numWildCardChars = line.Count(e => e == wildCard);

            if (numWildCardChars == 0)
                return line;

            if (numWildCardChars % 2 == 0)
                return FindAndReplaceWildCards(line, wildCard);

            throw new Exception($"On line '{line}', wildcards are not balanced");
        }

        private static string FindAndReplaceWildCards(string line, char wildCard)
        {
            var sVariables = line.Split(wildCard);

            var iVar = 0;
            var resultReplace = string.Empty;
            foreach (var variable in sVariables)
            {
                if (iVar % 2 == 0)
                {
                    resultReplace += variable;
                }
                else
                {
                    var envVar = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
                    if (string.IsNullOrEmpty(envVar))
                    {
                        throw new Exception($"Value or enviroment value '{variable}' not found");
                    }
                    resultReplace += envVar;
                }
                iVar++;
            }

            return resultReplace;
        }

        public static TimeSpan ToTimeSpan(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Property has not value");

            value = value.Trim();

            if (value.Length != 5)
                throw new Exception($"Length is incorrect on value {value}, value should has length (5), format 'HH:MM'");

            try
            {
                return new TimeSpan(int.Parse(value.Substring(0, 2)), int.Parse(value.Substring(3, 2)), 0);
            }
            catch (Exception)
            {
                throw new Exception($"Incorrect format on value {value}, must be HH:MM with 24 hours format");
            }

        }

        public static int ToInt(this string value)
        {
            int iValue;
            return int.TryParse(value, out iValue) ? iValue : 0;
        }
        public static decimal ToDecimal(this string value)
        {
            decimal iValue;
            return decimal.TryParse(value, out iValue) ? iValue : 0;
        }

        public static string IsCurrentDate(object year, object month, object day)
        {
            try
            {
                return new DateTime((int)year, (int)(month), (int)day) == DateTime.Today ? "YES" : "NO";
            }
            catch //
            {
                return "NO";
            }
        }
    }

}
