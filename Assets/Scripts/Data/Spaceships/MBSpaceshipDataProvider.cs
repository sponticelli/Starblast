using UnityEngine;

namespace Starblast.Data.Spaceships
{
    public class MBSpaceshipDataProvider : MonoBehaviour, ISpaceshipDataProvider
    {
        [SerializeField] private SpaceshipDataSO _spaceshipData;
        public ISpaceshipData Data => _spaceshipData;
    }
}