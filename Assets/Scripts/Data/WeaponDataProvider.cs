using UnityEngine;

namespace Starblast.Data
{
    public class WeaponDataProvider : MonoBehaviour, IWeaponDataProvider
    {
        [SerializeField] private WeaponDataSO _weaponData;
        public IWeaponData WeaponData => _weaponData;
    }
}