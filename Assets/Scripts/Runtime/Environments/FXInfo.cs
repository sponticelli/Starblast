using System;
using UnityEngine;

namespace Starblast.Environments
{
    [Serializable]
    public class FXInfo
    {
        public FXTypes type;
        public GameObject[] prefabs;
        public int initialPoolSize = 5;
        public int maxPoolSize = 10;
    }
}