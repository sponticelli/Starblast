using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starblast.Services
{
    public class GameObjectRegistry : MonoBehaviour, IInitializable
    {
        // Dictionary to hold registered objects
        private Dictionary<Type, List<MonoBehaviour>> _registry = new Dictionary<Type, List<MonoBehaviour>>();
        
        // Event to be called when a game object is registered
        public event Action<Type, MonoBehaviour> OnRegistered = delegate { };
        // Event to be called when a game object is deregistered
        public event Action<Type, MonoBehaviour> OnDeRegistered = delegate { };
        
        // Register a game object
        public void Register(MonoBehaviour obj)
        {
            var type = obj.GetType();
            if (!_registry.ContainsKey(type))
            {
                _registry[type] = new List<MonoBehaviour>();
            }
            _registry[type].Add(obj);
            OnRegistered?.Invoke(type, obj);
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
            OnDeRegistered?.Invoke(type, obj);
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

        public bool IsInitialized { get;  private set; }
        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }
    }
}