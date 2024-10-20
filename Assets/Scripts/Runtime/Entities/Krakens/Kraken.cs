using System;
using System.Collections.Generic;
using Starblast.Entities.Tentacles;
using Starblast.Player;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Entities.Krakens
{
    public class Kraken : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Tentacle> tentacles = new List<Tentacle>();
        
        private PlayerController _player;

        public void OnPlayerAdded(PlayerController player)
        {
            _player = player;
        }
        
        public void OnPlayerRemoved(PlayerController player)
        {
            _player = null;
        }
 

    }
}