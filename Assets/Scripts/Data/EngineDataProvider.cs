using Starblast.Actors;
using UnityEngine;

namespace Starblast.Data
{
    public class EngineDataProvider : MonoBehaviour, IEngineDataProvider
    {
        [SerializeField] private EngineDataSO _engineData;
        public IEngineData EngineData => _engineData;
    }
}