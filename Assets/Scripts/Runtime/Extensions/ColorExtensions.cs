using UnityEngine;

namespace Starblast.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color color)
        {
            return $"#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}";
        }
        
        public static string Color(this string text, Color color)
        {
           string output = $"<color=#{color.ToHex()}>{text}</color>";
           return output;
        }
    }
}