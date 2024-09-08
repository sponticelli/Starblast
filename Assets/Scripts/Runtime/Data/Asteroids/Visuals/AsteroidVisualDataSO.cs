using UnityEngine;

namespace Starblast.Data.Asteroids.Visuals
{
    [CreateAssetMenu(fileName = "NewAsteroidVisualData", menuName = "Starblast/Asteroid/Data/Visual Data")]
    public class AsteroidVisualDataSO : ScriptableObject, IAsteroidVisualData
    {
        [SerializeField] private Sprite sprite;

        public Sprite Sprite => sprite;
    }
}