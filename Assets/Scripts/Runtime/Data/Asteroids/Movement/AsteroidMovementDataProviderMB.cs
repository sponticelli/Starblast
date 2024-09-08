using UnityEngine;

namespace Starblast.Data.Asteroids.Movements
{
    [AddComponentMenu("Starblast/Asteroid/Data/Movement Data Provider")]
    public class AsteroidMovementDataProviderMB : MonoBehaviour, IAsteroidMovementDataProvider
    {
        [field: SerializeField] private AsteroidMovementDataSO _data;
        
        public IAsteroidMovementData Data => _data;
    }
}