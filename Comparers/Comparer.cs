
namespace CrackSoft.RegExplore.Comparers
{
    abstract class Comparer
    {
        public bool IgnoreCase { get; private set; }
        public string Pattern { get; private set; }        

        protected Comparer(string pattern, bool ignoreCase)
        {
            Pattern = pattern;
            IgnoreCase = ignoreCase;
        }

        public abstract bool IsMatch(string value);
    }
}
