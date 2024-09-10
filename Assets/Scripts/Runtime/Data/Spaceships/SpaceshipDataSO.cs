using Starblast.Actors.Movements;
using Starblast.Data.Spaceships.Visuals;
using Starblast.Data.Spaceships.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Data.Spaceships
{
    [CreateAssetMenu(fileName = "NewSpaceshipData", menuName = "Starblast/Spaceship/Spaceship Data")]
    public class SpaceshipDataSO : ScriptableObject, ISpaceshipData
    {
        [Header("Movement")]
        [SerializeField] private BodyDataSO bodyData;
        [SerializeField] private PropulsorDataSO propulsorData;
        
        
        [SerializeField] private WeaponDataSO _weaponData;
        [SerializeField] private SpaceshipVisualDataSO _visualData;

        public IBodyData BodyData => bodyData;
        public IPropulsorData PropulsorData => propulsorData;
        
        public IWeaponData WeaponData => _weaponData;
        public ISpaceshipVisualData VisualData => _visualData;
    }
}