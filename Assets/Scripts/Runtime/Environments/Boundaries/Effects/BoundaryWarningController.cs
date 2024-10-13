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


        [Header("References")] [SerializeField]
        private BoundaryText[] _boundaryTexts;


        private IBoundaryManager _boundaryManager;
        private PlayerRuntimeSet _playerRuntimeSet;
        private ShipController _player;

        private void Start()
        {
            _boundaryManager = ServiceLocator.Main.Get<IBoundaryManager>();
            _playerRuntimeSet = ServiceLocator.Main.Get<PlayerRuntimeSet>();
            _playerRuntimeSet.OnItemRegistered += OnPlayerAdded;
            _playerRuntimeSet.OnItemUnregistered += OnPlayerRemoved;
            _player = _playerRuntimeSet.GetPlayer();
            Initialize();
        }

        private void OnEnable()
        {
            if (_playerRuntimeSet != null)
            {
                _playerRuntimeSet.OnItemRegistered -= OnPlayerAdded;
                _playerRuntimeSet.OnItemUnregistered -= OnPlayerRemoved;
                _playerRuntimeSet.OnItemRegistered += OnPlayerAdded;
                _playerRuntimeSet.OnItemUnregistered += OnPlayerRemoved;
            }
        }

        private void OnDisable()
        {
            if (_playerRuntimeSet != null)
            {
                _playerRuntimeSet.OnItemRegistered -= OnPlayerAdded;
                _playerRuntimeSet.OnItemUnregistered -= OnPlayerRemoved;
            }
        }

        private void OnPlayerRemoved(ShipController player)
        {
            _player = null;
        }

        private void OnPlayerAdded(ShipController player)
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