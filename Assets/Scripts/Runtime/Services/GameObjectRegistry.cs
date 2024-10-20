using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starblast.Services
{
    public class GameObjectRegistry : MonoBehaviour, IInitializable
    {
        // Dictionary to hold registered objects
        private Dictionary<Type, List<MonoBehaviour>> _registry = new Dictionary<Type, List<MonoBehaviour>>();

        // Event to be called when any game object is registered
        public event Action<Type, MonoBehaviour> OnRegistered = delegate { };

        // Event to be called when any game object is deregistered
        public event Action<Type, MonoBehaviour> OnDeRegistered = delegate { };

        // Dictionary for type-specific registration events
        private Dictionary<Type, Action<MonoBehaviour>> _typeRegisteredEvents = new Dictionary<Type, Action<MonoBehaviour>>();
        private Dictionary<Type, Action<MonoBehaviour>> _typeDeRegisteredEvents = new Dictionary<Type, Action<MonoBehaviour>>();

        // Register a game object
        public void Register(MonoBehaviour obj)
        {
            var type = obj.GetType();
            if (!_registry.ContainsKey(type))
            {
                _registry[type] = new List<MonoBehaviour>();
            }
            _registry[type].Add(obj);

            // Invoke global event
            OnRegistered?.Invoke(type, obj);

            // Invoke type-specific event if present
            if (_typeRegisteredEvents.ContainsKey(type))
            {
                _typeRegisteredEvents[type]?.Invoke(obj);
            }
        }

        // Deregister a game object
        public void Deregister(MonoBehaviour obj)
        {
            var type = obj.GetType();
            if (!_registry.ContainsKey(type)) return;
            _registry[type].Remove(obj);
            if (_registry[type].Count == 0)
            {
                _registry.Remove(type);
            }

            // Invoke global event
            OnDeRegistered?.Invoke(type, obj);

            // Invoke type-specific event if present
            if (_typeDeRegisteredEvents.ContainsKey(type))
            {
                _typeDeRegisteredEvents[type]?.Invoke(obj);
            }
        }

        // Get a single instance (e.g., Player)
        public T Get<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_registry.ContainsKey(type) && _registry[type].Count > 0)
            {
                return _registry[type][0] as T;
            }
            return null;
        }

        // Get all instances (e.g., Enemies)
        public List<T> GetAll<T>() where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!_registry.ContainsKey(type)) return new List<T>();
            List<T> result = new List<T>();
            foreach (var obj in _registry[type])
            {
                result.Add(obj as T);
            }
            return result;
        }

        // Subscribe to specific type registration
        public void SubscribeToRegister<T>(Action<T> callback) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!_typeRegisteredEvents.ContainsKey(type))
            {
                _typeRegisteredEvents[type] = (obj) => callback((T)obj);
            }
            else
            {
                _typeRegisteredEvents[type] += (obj) => callback((T)obj);
            }
        }

        // Unsubscribe from specific type registration
        public void UnsubscribeFromRegister<T>(Action<T> callback) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_typeRegisteredEvents.ContainsKey(type))
            {
                _typeRegisteredEvents[type] -= (obj) => callback((T)obj);
            }
        }

        // Subscribe to specific type deregistration
        public void SubscribeToDeRegister<T>(Action<T> callback) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (!_typeDeRegisteredEvents.ContainsKey(type))
            {
                _typeDeRegisteredEvents[type] = (obj) => callback((T)obj);
            }
            else
            {
                _typeDeRegisteredEvents[type] += (obj) => callback((T)obj);
            }
        }

        // Unsubscribe from specific type deregistration
        public void UnsubscribeFromDeRegister<T>(Action<T> callback) where T : MonoBehaviour
        {
            var type = typeof(T);
            if (_typeDeRegisteredEvents.ContainsKey(type))
            {
                _typeDeRegisteredEvents[type] -= (obj) => callback((T)obj);
            }
        }

        public bool IsInitialized { get; private set; }
        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }
    }
}