using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Actors
{
    public class ActorController : MonoBehaviour, IRigidbody2DProvider
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        private IActorInputHandler _inputHandler;
        
        public Rigidbody2D GetRigidbody2D()
        {
            return _rigidbody2D;
        }
    }
}