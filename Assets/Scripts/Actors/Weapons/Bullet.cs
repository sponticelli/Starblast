using UnityEngine;

namespace Starblast.Actors.Weapons
{
    public class Bullet : MonoBehaviour, IBullet
    {
        [Header("References")]
        [SerializeField] public Rigidbody2D _rigidbody;
        
        [Header("Configuration")]
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 2f;
        
        private float _currentLifeTime;
        private Vector2 _direction;
        private Vector2 _shipVelocity;
        
        public void Shoot(Vector3 direction, Vector2 shipVelocity)
        {
            _rigidbody.isKinematic = false;
            _shipVelocity = shipVelocity;
            _direction = direction.normalized;
            _currentLifeTime = _lifeTime;
            _rigidbody.velocity = _shipVelocity +  _direction * _speed;
        }

        
        private void Update()
        {
            _currentLifeTime -= Time.deltaTime;
            if (_currentLifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }
        
    }
}