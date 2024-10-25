using UnityEngine;

namespace Starblast.Utils
{
    [System.Serializable]
    public class FloatRange : Range<float>
    {
        public FloatRange(float min, float max) : base(min, max)
        {
            
        }
        
        public override float RandomValue()
        {
            return Random.Range(Min, Max);
        }
    }
}