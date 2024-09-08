using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Data.Spaceships.Bodies
{
    [AddComponentMenu("Starblast/Spaceship/Data/Body Data Provider")]
    public class MBSpaceshipBodyDataProvider : MonoBehaviour, ISpaceshipBodyDataProvider
    {
        [SerializeField] private SpaceshipBodyDataSO spaceshipBodyData;
        public ISpaceshipBodyData Data => spaceshipBodyData;
    }
}