using Starblast.Data.Spaceships.Bodies;
using Starblast.Data.Spaceships.Engines;
using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public interface ISpaceshipMovementControllerContext
    {
        ISpaceshipBodyData SpaceshipBodyData { get; }
        ISpaceshipEngineData SpaceshipEngineData { get; }
        
        
        Rigidbody2D Rigidbody2D { get; }
        IActorInputHandler InputHandler { get; }
    }
}