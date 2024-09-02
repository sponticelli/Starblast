using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Starblast.Inputs
{
    public class PlayerInputSpace : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput _input;

        public UnityEvent<float> OnRotate;
        public UnityEvent<float> OnThrust;
        public UnityEvent OnFire;
        
        [field:SerializeField]
        public float Rotation { get; private set; }
        
        [field:SerializeField]
        public float Thrust { get; private set; }
        
        private void OnEnable()
        { 
            _input.actions["Player/Thrust"].performed += Thrusting;
            _input.actions["Player/Thrust"].canceled += Thrusting;
            
            _input.actions["Player/Rotate"].performed += Turning;
            _input.actions["Player/Rotate"].canceled += Turning;
            
            _input.actions["Player/Fire"].performed += Firing;
        }
        
        private void OnDisable()
        {
            _input.actions["Player/Thrust"].performed -= Thrusting;
            _input.actions["Player/Thrust"].canceled -= Thrusting;
            
            _input.actions["Player/Rotate"].performed -= Turning;
            _input.actions["Player/Rotate"].canceled -= Turning;
            
            _input.actions["Player/Fire"].performed -= Firing;
        }

        private void Firing(InputAction.CallbackContext obj)
        {
            OnFire?.Invoke();
        }

        private void Turning(InputAction.CallbackContext obj)
        { 
            Rotation = obj.ReadValue<float>();
            OnRotate?.Invoke(Rotation);
        }

        private void Thrusting(InputAction.CallbackContext obj)
        {
            Thrust = obj.ReadValue<float>();
            OnThrust?.Invoke(Thrust);
        }
    }
}