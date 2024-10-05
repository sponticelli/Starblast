using UnityEngine;

namespace Starblast.Services
{
    [DefaultExecutionOrder(ExecutionOrder.ServiceLocatorStrapper)]
    public class ServiceLocatorStrapper : MonoBehaviour
    {
        [SerializeField] private ServiceLocator _serviceLocatorPrefab;
        
        private void Awake()
        {
            if (ServiceLocator.Main != null)
            {
                Destroy(gameObject);
                return;
            }
            var obj = Instantiate(_serviceLocatorPrefab);
            obj.name = _serviceLocatorPrefab.name;
        }
    }
}