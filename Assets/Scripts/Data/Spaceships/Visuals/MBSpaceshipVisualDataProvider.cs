using UnityEngine;

namespace Starblast.Data.Spaceships.Visuals
{
    [AddComponentMenu("Starblast/Spaceship/Data/Visual Data Provider")]
    public class MBSpaceshipVisualDataProvider : MonoBehaviour, ISpaceshipVisualDataProvider
    {
        [SerializeField] private SpaceshipVisualDataSO _spaceshipVisualData;
        public ISpaceshipVisualData Data => _spaceshipVisualData;

    }
}