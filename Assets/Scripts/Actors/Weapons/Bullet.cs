using UnityEngine;
using UnityEngine.Pool;

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
        private IObjectPool<Bullet> _bulletPool = null;
        
        private bool _isReleased = false;
        
        public void Shoot(Vector3 direction, Vector2 shipVelocity)
        {
            _rigidbody.isKinematic = false;
            _shipVelocity = shipVelocity;
            _direction = direction.normalized;
            _currentLifeTime = _lifeTime;
            _isReleased = false;
        }

        
        private void Update()
        {
            _currentLifeTime -= Time.deltaTime;
            if (_currentLifeTime <= 0 && !_isReleased)
            {
                _bulletPool.Release(this);
                _isReleased = true;
            }

        }
        
        private void FixedUpdate()
        {
            _rigidbody.velocity = (_direction * _speed) + _shipVelocity;
        }

        public void Initialize(IObjectPool<Bullet> bulletPool)
        {
            _bulletPool = bulletPool;
        }
    }
}