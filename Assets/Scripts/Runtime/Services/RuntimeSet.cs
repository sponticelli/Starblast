using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Starblast.Services
{
    /// <summary>
    /// Abstract generic class for managing a runtime set of items.
    /// </summary>
    /// <typeparam name="T">Type of items to manage.</typeparam>
    public abstract class RuntimeSet<T> : MonoBehaviour, IInitializable
    {
        protected HashSet<T> _items;
        
        public IReadOnlyCollection<T> Items => _items;
        
        public event Action<T> OnItemRegistered;
        public event Action<T> OnItemUnregistered;
        
        public bool IsInitialized { get; private set; }
        
        private void Awake()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            if (IsInitialized) return;
            _items = new HashSet<T>();
            IsInitialized = true;
        }
        
        public T GetFirst()
        {
            return _items.FirstOrDefault();
        }
        
        public void Register(T item)
        {
            if (item == null)
            {
                Debug.LogWarning("Attempted to register a null item.");
                return;
            }
            if (_items.Add(item)) OnItemRegistered?.Invoke(item);
        }
        
        public void Unregister(T item)
        {
            _items.Remove(item);
            OnItemUnregistered?.Invoke(item);
        }
    }
}