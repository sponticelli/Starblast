using UnityEngine;

namespace Starblast.Environments
{
    public class AutoPlayAnimation : MonoBehaviour
    {
        // Enum to choose when to start the animation
        public enum StartEvent
        {
            Awake,
            OnEnable,
            Start
        }

        // This variable can be set in the Unity Inspector to choose the event
        [SerializeField] private StartEvent startEvent = StartEvent.Start;
        [SerializeField] private Animator animator;

        private void Awake()
        {
            // Cache the Animator component
            if (animator == null) animator = GetComponent<Animator>();

            // Check if the selected event is Awake
            if (startEvent == StartEvent.Awake)
            {
                StartAnimation();
            }
        }

        private void OnEnable()
        {
            // Check if the selected event is OnEnable
            if (startEvent == StartEvent.OnEnable)
            {
                StartAnimation();
            }
        }

        private void Start()
        {
            // Check if the selected event is Start
            if (startEvent == StartEvent.Start)
            {
                StartAnimation();
            }
        }

        // Method to start the animation
        private void StartAnimation()
        {
            if (animator != null)
            {
                // Reset the animation to the start
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, 0f);
            }
            else
            {
                Debug.LogWarning("Animator component is missing on this GameObject.");
            }
        }
    }
}