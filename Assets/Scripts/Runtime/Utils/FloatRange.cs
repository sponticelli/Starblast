namespace Starblast.Utils
{
    [System.Serializable]
    public class FloatRange : Range<float>
    {
        public FloatRange(float min, float max) : base(min, max)
        {
        }
    }
}