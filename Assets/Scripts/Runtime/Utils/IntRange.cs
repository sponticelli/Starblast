namespace Starblast.Utils
{
    [System.Serializable]
    public class IntRange : Range<int>
    {
        public IntRange(int min, int max) : base(min, max)
        {
        }
    }
}