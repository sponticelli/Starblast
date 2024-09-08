using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Data.Spaceships.Visuals;
using Starblast.Data.Spaceships.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Data.Spaceships
{
    [CreateAssetMenu(fileName = "NewSpaceshipData", menuName = "Starblast/Spaceship/Spaceship Data")]
    public class SpaceshipDataSO : ScriptableObject, ISpaceshipData
    {
        [FormerlySerializedAs("_bodyData")] [SerializeField] private SpaceshipBodyDataSO spaceshipBodyData;
        [FormerlySerializedAs("_engineData")] [SerializeField] private SpaceshipEngineDataSO spaceshipEngineData;
        [SerializeField] private WeaponDataSO _weaponData;
        [SerializeField] private SpaceshipVisualDataSO _visualData;

        public ISpaceshipBodyData SpaceshipBodyData => spaceshipBodyData;
        public ISpaceshipEngineData SpaceshipEngineData => spaceshipEngineData;
        public IWeaponData WeaponData => _weaponData;
        public ISpaceshipVisualData VisualData => _visualData;
    }
}