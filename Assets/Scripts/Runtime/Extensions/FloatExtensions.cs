using UnityEngine;

namespace Starblast.Extensions
{
    public static class FloatExtensions
    {
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Max(min, Mathf.Min(max, value));
        }
        
        public static float Clamp01(this float value)
        {
            return Mathf.Max(0, Mathf.Min(1, value));
        }
        
        public static float Normalize(this float value,  float min = 0, float max = 1f) 
            => (value - min) / (max - min);

        
        public static float NormalizeAngle(this float ang)
        {
            ang = (ang + 180) % 360;
            if (ang < 0)
                ang += 360;
            return ang - 180;
        }
    }
}