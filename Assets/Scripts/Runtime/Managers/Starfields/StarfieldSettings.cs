using System;
using UnityEngine;

namespace Starblast.Managers.Starfields
{
    [Serializable]
    public class StarfieldSettings
    {
        [field:SerializeField] public float Speed { get; private set; }
        [field:SerializeField] public float Size { get; private set; }
        [field:SerializeField] public float Density { get; private set; }
        [field:SerializeField] public Color Color { get; private set; }
        [field:SerializeField] public float Offset { get; private set; }


        public StarfieldSettings(float speed, float size, float density, float offset, Color color)
        {
            Speed = speed;
            Size = size;
            Density = density;
            Color = color;
            Offset = offset;
        }
        
        public void CopyFrom(StarfieldSettings other)
        {
            Speed = other.Speed;
            Size = other.Size;
            Density = other.Density;
            Color = other.Color;
            Offset = other.Offset;
        }
    }
    
    
}