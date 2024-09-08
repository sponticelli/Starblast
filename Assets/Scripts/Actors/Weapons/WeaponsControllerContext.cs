using Starblast.Data;
using Starblast.Data.Spaceships.Weapons;
using Starblast.Inputs;

namespace Starblast.Actors.Weapons
{
    public class WeaponsControllerContext : IWeaponsControllerContext
    {
        public IVelocityProvider VelocityProvider { get; }
        public IActorInputHandler ActorInputHandler { get; }
        
        public IWeaponDataProvider WeaponDataProvider { get; }

        public WeaponsControllerContext(IVelocityProvider velocityProvider, 
            IActorInputHandler actorInputHandler, 
            IWeaponDataProvider weaponDataProvider)
        {
            VelocityProvider = velocityProvider;
            ActorInputHandler = actorInputHandler;
            WeaponDataProvider = weaponDataProvider;
        }
    }
}