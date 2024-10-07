using System;
using Starblast.Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Environments
{
    /// <summary>
    /// It teleports objects to the other side of the camera when they are far enough from the player.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class LevelBounds : MonoBehaviour, IInitializable
    {
        [Header("Settings")]
        [SerializeField] private float _innerRadius = 100f;
        [SerializeField] private float _outerRadius = 120f;

        
        public float InnerRadius => _innerRadius;
        public float OuterRadius => _outerRadius;
        
        public bool IsInitialized { get; private set; }
        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        private void OnDrawGizmos()
        {
            // orange
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); 
            Gizmos.DrawWireSphere(Vector3.zero, _innerRadius);
            
            // red
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(Vector3.zero, _outerRadius);
        }

        
    }
}