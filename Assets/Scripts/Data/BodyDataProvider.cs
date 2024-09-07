using Starblast.Actors;
using UnityEngine;

namespace Starblast.Data
{
    public class BodyDataProvider : MonoBehaviour, IBodyDataProvider
    {
        [SerializeField] private BodyDataSO _bodyData;
        public IBodyData BodyData => _bodyData;
    }
}