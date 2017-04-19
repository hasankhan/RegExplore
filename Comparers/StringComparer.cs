
namespace CrackSoft.RegExplore.Comparers
{
    class StringComparer: Comparer
    {
        string patternLower;
        
        public StringComparer(string pattern, bool ignoreCase) : base(pattern, ignoreCase)
        {
            if (ignoreCase)
                patternLower = pattern.ToLower();
        }

        public override bool IsMatch(string value)
        {
            if (IgnoreCase)
                return value.ToLower().Contains(patternLower);
            else
                return value.Contains(Pattern);
        }
    }
}
