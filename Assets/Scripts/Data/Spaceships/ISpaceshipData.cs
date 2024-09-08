using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Data.Spaceships.Visuals;
using Starblast.Data.Spaceships.Weapons;

namespace Starblast.Data.Spaceships
{
    public interface ISpaceshipData : IData
    {
        ISpaceshipBodyData SpaceshipBodyData { get; }
        ISpaceshipEngineData SpaceshipEngineData { get; }
        IWeaponData WeaponData { get; }
        ISpaceshipVisualData VisualData { get; }
    }
}