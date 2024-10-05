using System;

namespace Starblast.Entities.Asteroids
{
    [Serializable]
    public class AsteroidInfo
    {
        public AsteroidSize size;
        public Asteroid[] prefabs;
        public int initialPoolSize = 10;
        public int maxPoolSize = 100;
    }
}