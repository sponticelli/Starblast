using UnityEngine;

namespace Starblast.Data.Spaceships.Weapons
{
    [AddComponentMenu("Starblast/Spaceship/Data/Weapon Data Provider")]
    public class MBWeaponDataProvider : MonoBehaviour, IWeaponDataProvider
    {
        [SerializeField] private WeaponDataSO _weaponData;
        public IWeaponData Data => _weaponData;
    }
    
}