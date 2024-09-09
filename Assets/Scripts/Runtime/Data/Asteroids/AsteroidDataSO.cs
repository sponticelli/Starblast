using Starblast.Data.Asteroids.Movements;
using Starblast.Data.Asteroids.Visuals;
using UnityEngine;

namespace Starblast.Data.Asteroids
{
    [CreateAssetMenu(fileName = "NewAsteroidData", menuName = "Starblast/Asteroid/Data/Asteroid Data")]
    public class AsteroidDataSO : ScriptableObject, IAsteroidData
    {
        public IAsteroidVisualData VisualData { get; }
        public IAsteroidMovementData MovementData { get; }
    }
}