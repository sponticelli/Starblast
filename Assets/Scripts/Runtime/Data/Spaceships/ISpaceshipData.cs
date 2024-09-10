using Starblast.Actors.Movements;
using Starblast.Data.Spaceships.Visuals;
using Starblast.Data.Spaceships.Weapons;

namespace Starblast.Data.Spaceships
{
    public interface ISpaceshipData : IData
    {
        IBodyData BodyData { get; }
        IPropulsorData PropulsorData { get; }
        
        IWeaponData WeaponData { get; }
        ISpaceshipVisualData VisualData { get; }
    }
}