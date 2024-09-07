using UnityEngine;

namespace Starblast.Data
{
    public class SpaceshipVisualDataProvider : MonoBehaviour, ISpaceshipVisualDataProvider
    {
        [SerializeField] private SpaceshipVisualDataSO _spaceshipVisualData;
        public ISpaceshipVisualData VisualData => _spaceshipVisualData;

    }
    

}