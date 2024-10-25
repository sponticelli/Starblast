using UnityEngine;

namespace Starblast.Utils
{
    [System.Serializable]
    public class IntRange : Range<int>
    {
        public IntRange(int min, int max) : base(min, max)
        {
        }
        
        public override int RandomValue()
        {
            return Random.Range(Min, Max);
        }
    }
}