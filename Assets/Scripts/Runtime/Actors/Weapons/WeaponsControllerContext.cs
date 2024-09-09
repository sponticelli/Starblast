using Starblast.Data.Spaceships.Weapons;
using Starblast.Inputs;

namespace Starblast.Actors.Weapons
{
    public class WeaponsControllerContext : IWeaponsControllerContext
    {
        public IVelocityProvider VelocityProvider { get; }
        public IActorInputHandler ActorInputHandler { get; }
        
        public IWeaponData WeaponData { get; }

        public WeaponsControllerContext(IVelocityProvider velocityProvider, 
            IActorInputHandler actorInputHandler, 
            IWeaponData weaponData)
        {
            VelocityProvider = velocityProvider;
            ActorInputHandler = actorInputHandler;
            WeaponData = weaponData;
        }
    }
}