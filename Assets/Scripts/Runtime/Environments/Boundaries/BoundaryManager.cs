using Starblast.Extensions;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    [DefaultExecutionOrder(ExecutionOrder.Services)]
    public class BoundaryManager : MonoBehaviour, IBoundaryManager
    {
        [SerializeField] private float _safeZoneRadius = 100f;
        [SerializeField] private float _warningZoneRadius = 200f;
        [SerializeField] private float _dangerZoneRadius = 300f;
        [SerializeField] private float _deadZoneRadius = 400f;
        
        public bool IsInitialized { get; private set; }
        public void Initialize()
        {
            // DO NOTHING
            IsInitialized = true;
        }

        public ZoneType GetZoneType(Vector3 position)
        {
            float distance = position.magnitude;
            if (distance < _safeZoneRadius)
            {
                return ZoneType.SafeZone;
            }

            if (distance < _warningZoneRadius)
            {
                return ZoneType.WarningZone;
            }

            if (distance < _dangerZoneRadius)
            {
                return ZoneType.DangerZone;
            }
            return ZoneType.DeadZone;
        }

        public float GetZoneRadius(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.SafeZone:
                    return _safeZoneRadius;
                case ZoneType.WarningZone:
                    return _warningZoneRadius;
                case ZoneType.DangerZone:
                    return _dangerZoneRadius;
                case ZoneType.DeadZone:
                    return _deadZoneRadius;
                default:
                    return 0f;
            }
        }

        public float GetDistanceToZone(Vector3 position, ZoneType zoneType)
        {
            float distance = position.Magnitude2D();
            float radius = GetZoneRadius(zoneType);
            return radius - distance;
        }

        public float NormalizedPositionInZone(Vector3 position, ZoneType zoneType)
        {
            float distance = position.Magnitude2D();
            float innerRadius = zoneType == ZoneType.SafeZone ? 0f :  GetZoneRadius(zoneType - 1);
            return (distance - innerRadius) / (GetZoneRadius(zoneType) - innerRadius);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, _safeZoneRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Vector3.zero, _warningZoneRadius);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Vector3.zero, _dangerZoneRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, _deadZoneRadius);
        }
    }
}