using UnityEngine;

namespace Starblast.Data.Spaceships.Engines
{
    [AddComponentMenu("Starblast/Spaceship/Data/Engine Data Provider")]
    public class MBSpaceshipEngineDataProvider : MonoBehaviour, ISpaceshipEngineDataProvider
    {
        [SerializeField] private SpaceshipEngineDataSO spaceshipEngineData;
        public ISpaceshipEngineData Data => spaceshipEngineData;
    }
}