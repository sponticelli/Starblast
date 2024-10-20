using System;
using Starblast.Player;
using Starblast.Services;
using TMPro;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    public class BoundaryWarningController : MonoBehaviour, IBoundaryWarningController
    {
        [Serializable]
        public class BoundaryText
        {
            public ZoneType zone;
            public TextMeshPro label;
            public string[] messages;
        }


        [Header("References")] 
        [SerializeField] private BoundaryText[] _boundaryTexts;


        private IBoundaryManager _boundaryManager;
        private PlayerController _player;

        private void Start()
        {
            Initialize();
        }

        
        public void OnPlayerRemoved(PlayerController player)
        {
            _player = null;
        }

        public void OnPlayerAdded(PlayerController player)
        {
            _player = player;
        }


        public void ResetEffects()
        {
        }

        public void OnEnterZone(ZoneType zone)
        {
        }

        public void OnExitZone(ZoneType zone)
        {
        }

        public void SetEffectIntensity(float intensity)
        {
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            var normalizedVector = _player.transform.position.normalized;
            // Calc the rotation on the z axis around the origin
            float angle = -90 + Mathf.Atan2(normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;
            // rotate the object on the z axis around the origin (0,0,0)
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void Initialize()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            
            foreach (var boundaryText in _boundaryTexts)
            {
                ZoneType zone = boundaryText.zone;
                float radius = _boundaryManager.GetZoneRadius(zone);
                boundaryText.label.text =
                    boundaryText.messages[UnityEngine.Random.Range(0, boundaryText.messages.Length)];
                boundaryText.label.transform.position = new Vector3(0, radius, 0);
            }
        }
    }
}