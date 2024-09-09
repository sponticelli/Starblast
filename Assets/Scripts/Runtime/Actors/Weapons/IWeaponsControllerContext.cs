using Starblast.Data.Spaceships.Weapons;
using Starblast.Inputs;

namespace Starblast.Actors.Weapons
{
    public interface IWeaponsControllerContext
    {
        public IVelocityProvider VelocityProvider { get; }
        public IActorInputHandler ActorInputHandler { get; }
        
        public IWeaponData WeaponData { get; }
    }
}