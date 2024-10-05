using UnityEngine;

namespace Starblast.Entities.Asteroids
{
    [CreateAssetMenu(fileName = "AsteroidData", menuName = "Starblast/Data/Entities/Asteroid")]
    public class AsteroidData : ScriptableObject
    {
        public AsteroidSize size;
        public int health;
        
        [Header("Splitting")]
        public AsteroidSplittingInfo[] splittingInfo;
        
        [Header("Physics")]
        public float speed;
        public float mass;
        public float rotationSpeed = 1f;
        
    }
}