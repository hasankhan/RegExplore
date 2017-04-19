using System;
using System.Text.RegularExpressions;

namespace CrackSoft.RegExplore.Comparers
{
    class RegexComparer: Comparer
    {
        Regex regEx;

        public RegexComparer(string pattern, bool ignoreCase) : base(pattern, ignoreCase)
        {
            RegexOptions opts = RegexOptions.Compiled;
            if (IgnoreCase)
                opts |= RegexOptions.IgnoreCase;

            try
            {
                regEx = new Regex(Pattern, opts);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid regular expression", ex);
            }
        }

        public override bool IsMatch(string value)
        {
            return regEx.IsMatch(value);
        }
    }
}
