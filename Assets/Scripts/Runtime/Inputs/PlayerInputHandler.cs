using UnityEngine;
using UnityEngine.InputSystem;

namespace Starblast.Inputs
{
    public class PlayerInputHandler : MonoBehaviour, IActorInputHandler
    {
        [Header("References")]
        [SerializeField]
        private PlayerInput _input;
        
        [field:Header("Properties")]
        [field:SerializeField]
        public float Rotation { get; private set; }
        [field:SerializeField]
        public float Thrust { get; private set; }
        
        public event InputEvent<float> OnRotate;
        public event InputEvent<float> OnThrust;
        public event InputEvent OnFirePressed;
        public event InputEvent OnFireReleased;
        
        private void OnEnable()
        { 
            _input.actions["Player/Thrust"].performed += Thrusting;
            _input.actions["Player/Thrust"].canceled += Thrusting;
            
            _input.actions["Player/Rotate"].performed += Turning;
            _input.actions["Player/Rotate"].canceled += Turning;
            
            _input.actions["Player/Fire"].performed += Firing;
            _input.actions["Player/Fire"].canceled += Firing;
        }
        
        private void OnDisable()
        {
            _input.actions["Player/Thrust"].performed -= Thrusting;
            _input.actions["Player/Thrust"].canceled -= Thrusting;
            
            _input.actions["Player/Rotate"].performed -= Turning;
            _input.actions["Player/Rotate"].canceled -= Turning;
            
            _input.actions["Player/Fire"].performed -= Firing;
            _input.actions["Player/Fire"].canceled -= Firing;
        }

        private void Thrusting(InputAction.CallbackContext obj)
        {
            Thrust = obj.ReadValue<float>();
            OnThrust?.Invoke(Thrust);
        }

        private void Turning(InputAction.CallbackContext obj)
        {
            Rotation = obj.ReadValue<float>();
            OnRotate?.Invoke(Rotation);
        }

        private void Firing(InputAction.CallbackContext obj)
        {
            var isPressed = obj.ReadValue<float>() > 0.5f;
            if (isPressed)
            {
                OnFirePressed?.Invoke();
            }
            else
            {
                OnFireReleased?.Invoke();
            }
        }
    }
}