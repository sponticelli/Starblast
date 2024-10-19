using UnityEngine;

namespace Starblast.Extensions
{
    public static class Vector3Extensions
    {
        public static float Magnitude2D(this Vector3 vec)
        {
            return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
        }

        public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, float angle) =>
            Quaternion.Euler( Vector3.forward * angle) * (point - pivot) + pivot;
        
        
    }
}