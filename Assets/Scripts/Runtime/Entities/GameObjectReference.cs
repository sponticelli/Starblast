using Starblast.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Starblast.Entities
{
    public class GameObjectReference<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected GameObjectRegistry _gameObjectRegistry;
        protected T _item;
        protected bool _exists;
        
        public bool Exists => _exists;
        public T Item => _item;
        
        public UnityEvent<T> onAddedEvent = new UnityEvent<T>();
        public UnityEvent<T> onRemovedEvent = new UnityEvent<T>();
        
        protected void Start()
        {
            _gameObjectRegistry = ServiceLocator.Main.Get<GameObjectRegistry>();
            Subscribe();
        }

        private void OnEnable()
        {
            if (_gameObjectRegistry == null) return;
            
            Unsubscribe();
            Subscribe();
            
        }
        
        private void OnDisable()
        {
            if (_gameObjectRegistry == null) return;
            Unsubscribe();
        }
        
        protected void OnAdded(T item)
        {
            _item = item;
            _exists = true;
        }
        
        protected void OnRemoved(T item)
        {
            _item = null;
            _exists = false;
        }
        
        private void Subscribe()
        {
            _item = _gameObjectRegistry.Get<T>();
            _exists = _item != null;
            _gameObjectRegistry = ServiceLocator.Main.Get<GameObjectRegistry>();
            _gameObjectRegistry.SubscribeToRegister<T>(OnAdded);
            _gameObjectRegistry.SubscribeToDeRegister<T>(OnRemoved);
            
            if (_exists)
            {
                onAddedEvent.Invoke(_item);
            }
            else
            {
                onRemovedEvent.Invoke(_item);
            }
        }
        
        private void Unsubscribe()
        {
            _gameObjectRegistry.UnsubscribeFromRegister<T>(OnAdded);
            _gameObjectRegistry.UnsubscribeFromDeRegister<T>(OnRemoved);
        }
    }
}