using Starblast.Data;
using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors.Movements
{
    public interface ISpaceshipMovementControllerContext
    {
        IBodyData BodyData { get; }
        IPropulsorData PropulsorData { get; }
        
        
        Rigidbody2D Rigidbody2D { get; }
        IActorInputHandler InputHandler { get; }
    }
}