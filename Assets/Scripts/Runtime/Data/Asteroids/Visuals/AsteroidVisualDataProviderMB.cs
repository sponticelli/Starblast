using UnityEngine;

namespace Starblast.Data.Asteroids.Visuals
{
    [AddComponentMenu("Starblast/Asteroid/Data/Visual Data Provider")]
    public class AsteroidVisualDataProviderMB : MonoBehaviour, IAsteroidVisualDataProvider
    {
        [field: SerializeField] private AsteroidVisualDataSO _asteroidVisualData;
        
        public IAsteroidVisualData Data => _asteroidVisualData;
    }
}