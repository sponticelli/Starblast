using Starblast.Services;
using UnityEngine;

namespace Starblast.Environments.Boundaries
{
    public interface IBoundaryManager : IInitializable
    {
        public ZoneType GetZoneType(Vector3 position);
        public float GetZoneRadius(ZoneType zoneType);
        public float GetDistanceToZone(Vector3 position, ZoneType zoneType);
        public float GetNormalizedDistanceToZone(Vector3 position, ZoneType zoneType);
    }
}