namespace Starblast.Data.Spaceships.Weapons
{
    public class WeaponDataProvider : IWeaponDataProvider
    {
        public IWeaponData Data { get; }

        public WeaponDataProvider(IWeaponData weaponData)
        {
            Data = weaponData;
        }
    }
}