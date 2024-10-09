using Starblast.Environments;
using Starblast.Environments.Boundaries;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.Asteroids
{
    public class AsteroidMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private float speed;
        private float rotationSpeed;
        
        public void Initialize(Rigidbody2D rigidbody,  float speed, float rotationSpeed)
        {
            rb = rigidbody;
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            StartMovement();
        }

        private void StartMovement()
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.velocity = randomDirection * speed;
            
            float randomRotation = Random.Range(-rotationSpeed, rotationSpeed);
            rb.angularVelocity = randomRotation;
        }
        
        
    }
    
}