using UnityEngine;

namespace Starblast.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// t calculates a point on a cubic Bezier curve defined by four control points (p0, p1, p2, p3) at a given parameter t.
        /// </summary>
        public static Vector2 GetCubicBezierPoint(this Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1 - t;
            return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
        }

        public static Vector2 GetPerpendicular(this Vector2 from, Vector2 to, float side = 1f)
        {
            var direction = from - to;
            return new Vector2(direction.y * side, -direction.x * side);
        }
        
        public static Vector2 GetPerpendicularNoAlloc(this Vector2 from, Vector2 to, ref Vector2 result, float side = 1f)
        {
            var direction = from - to;
            result.x = direction.y * side;
            result.y = -direction.x * side;
            return result;
        }
        
    }
}