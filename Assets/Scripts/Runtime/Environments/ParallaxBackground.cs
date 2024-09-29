using UnityEngine;

namespace Starblast.Environments
{
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Parallax Settings")]
        public Transform target;           // The player's Transform
        public float parallaxSpeed = 0.5f; // The speed at which the background moves

        private Vector3 previousTargetPosition;

        void Start()
        {
            if (target == null)
            {
                Debug.LogError("ParallaxBackground script requires a target to function.");
                return;
            }

            // Store the initial position of the target
            previousTargetPosition = target.position;
        }

        void Update()
        {
            if (target == null) return;

            // Calculate the difference in the target's position since the last frame
            Vector3 deltaMovement = target.position - previousTargetPosition;

            // Move the background opposite to the target's movement, scaled by parallaxSpeed
            transform.position -= deltaMovement * parallaxSpeed;

            // Update the previous target position for the next frame
            previousTargetPosition = target.position;
        }
    }
}