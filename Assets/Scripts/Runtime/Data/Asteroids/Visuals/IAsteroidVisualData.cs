using UnityEngine;

namespace Starblast.Data.Asteroids.Visuals
{
    public interface IAsteroidVisualData : IData
    {
        public Sprite Sprite { get; }
    }
}