using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Environments
{
    /// <summary>
    /// It teleports objects to the other side of the camera when they are far enough from the player.
    /// </summary>
    public class LevelBounds : MonoBehaviour, IInitializable
    {
        [Header("Settings")]
        [SerializeField] private Vector2 _size = new Vector2(800, 800);
        [SerializeField] private float _teleportFraction = 0.25f;
        
        [Header("References")]
        [SerializeField] private Camera _camera;

        private Vector2 _mins;
        private Vector2 _maxs;


        private void Start()
        {
            Initialize();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _size);
        }

        public bool IsInitialized { get; private set;  }
        public void Initialize()
        {
            var position = transform.position;
            _mins = new Vector2(position.x - 0.5f * _size.x, position.y - 0.5f * _size.y);
            _maxs = new Vector2(position.x + 0.5f * _size.x, position.y + 0.5f * _size.y);

            IsInitialized = true;
        }

        public Vector3 CalcLoopingPosition(Vector3 position, bool resetOutOfBounds = false)
        {
            float levelWidth = _size.x;
            float levelHeight = _size.y;

            // Horizontal looping
            if (IsOnLeftSide(position.x))
            {
                position.x += levelWidth;
            }
            else if (IsOnRightSide(position.x))
            {
                position.x -= levelWidth;
            }
            else if (resetOutOfBounds)
            {
                if (position.x < _mins.x && !IsOnLeftSide(GetCameraX(), true))
                {
                    position.x += levelWidth;
                }
                else if (position.x > _maxs.x && !IsOnRightSide(GetCameraX(), true))
                {
                    position.x -= levelWidth;
                }
            }

            // Vertical looping
            if (IsOnBottomSide(position.y))
            {
                position.y += levelHeight;
            }
            else if (IsOnTopSide(position.y))
            {
                position.y -= levelHeight;
            }
            else if (resetOutOfBounds)
            {
                if (position.y < _mins.y && !IsOnBottomSide(GetCameraY(), true))
                {
                    position.y += levelHeight;
                }
                else if (position.y > _maxs.y && !IsOnTopSide(GetCameraY(), true))
                {
                    position.y -= levelHeight;
                }
            }

            return position;
        }
        
        public Vector3 CalcPositionIfOutOfBounds(Vector3 position)
        {
            if (!IsInitialized)
            {
                Debug.LogError("LevelBounds not initialized. Please call Initialize before use.");
                return position;
            }

            if (position.x < _mins.x)
            {
                position.x = _maxs.x;
            }
            else if (position.x > _maxs.x)
            {
                position.x = _mins.x;
            }

            if (position.y < _mins.y)
            {
                position.y = _maxs.y;
            }
            else if (position.y > _maxs.y)
            {
                position.y = _mins.y;
            }

            return position;
        }
        
        public float GetCameraX()
        {
            return _camera.transform.position.x;
        }

        public float GetCameraY()
        {
            return _camera.transform.position.y;
        }

        // object on left side, PLAYER is on right side
        private bool IsOnLeftSide( float x, bool isCamera = false )
        {
            if ( isCamera )
                return x < (_mins.x + _size.x * _teleportFraction);

            return IsOnRightSide( GetCameraX(), true ) && x < (_mins.x + _size.x * _teleportFraction); 
        }

        // object on right side, PLAYER is on left side
        private bool IsOnRightSide(float x, bool isCamera = false)
        {
            if (isCamera)
                return x > (_maxs.x - _size.x * _teleportFraction);

            return IsOnLeftSide( GetCameraX(), true ) && x > (_maxs.x - _size.x * _teleportFraction);
        }
        
        // object on top side, PLAYER is on bottom side
        private bool IsOnTopSide(float y, bool isCamera = false)
        {
            if (isCamera)
                return y > (_maxs.y - _size.y * _teleportFraction);

            return IsOnBottomSide(GetCameraY(), true) && y > (_maxs.y - _size.y * _teleportFraction);
        }

// object on bottom side, PLAYER is on top side
        private bool IsOnBottomSide(float y, bool isCamera = false)
        {
            if (isCamera)
                return y < (_mins.y + _size.y * _teleportFraction);

            return IsOnTopSide(GetCameraY(), true) && y < (_mins.y + _size.y * _teleportFraction);
        }

    }
}