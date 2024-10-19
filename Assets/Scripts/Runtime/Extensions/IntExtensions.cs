using UnityEngine;

namespace Starblast.Extensions
{
    public static class IntExtensions
    {
        public static int Clamp(this int value, int min, int max)
        {
            return Mathf.Max(min, Mathf.Min(max, value));
        }
        
        public static int Clamp01(this int value)
        {
            return Mathf.Max(0, Mathf.Min(1, value));
        }
        
        public static int Normalize(this int value,  int min = 0, int max = 1) 
            => (value - min) / (max - min);
    }
}