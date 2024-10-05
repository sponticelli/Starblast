using System;

namespace Starblast.Entities.Asteroids
{
    [Serializable]
    public class AsteroidSplittingInfo
    {
        public AsteroidSize size;
        public int minSplitCount;
        public int maxSplitCount;
        
        public float spawnRadius = 0.5f;
    }
}