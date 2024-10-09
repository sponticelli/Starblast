using System;
using Starblast.Entities;
using Starblast.Environments;
using Starblast.Environments.Boundaries;
using Starblast.Pools;
using Starblast.Services;
using UnityEngine;
using UnityEngine.Events;


namespace Starblast.Weapons
{
    public class Bullet : MonoBehaviour, IBullet
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private PooledObject _pooledObject;
        
        [Header("Configuration")]
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 2f;
        [SerializeField] private float _damage = 1f;
        
        [Header("Events")]
        [SerializeField] private UnityEvent _onBulletHit;
        
        private float _currentLifeTime;
        private Vector2 _direction;
        private Vector2 _shipVelocity;
        
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
                Die();
            }

        }

        private void Die()
        {
            _isReleased = true;
            if (_pooledObject == null) _pooledObject = GetComponent<PooledObject>();
            _pooledObject.ReturnToPool();
        }

        private void FixedUpdate()
        {
            _rigidbody.velocity = (_direction * _speed) + _shipVelocity;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isReleased) return;
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
            _onBulletHit?.Invoke();
            Die();
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isReleased) return;
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
            Die();
        }
        
        
    }
    
    
}