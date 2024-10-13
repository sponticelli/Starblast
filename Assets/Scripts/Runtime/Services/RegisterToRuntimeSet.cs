using UnityEngine;

namespace Starblast.Services
{
    [DefaultExecutionOrder(ExecutionOrder.RegisterToRuntimeSet)]
    public class RegisterToRuntimeSet<TItem, TRuntimeSet> : MonoBehaviour
        where TRuntimeSet : RuntimeSet<TItem>
    {
        private TRuntimeSet _runtimeSet;
        private TItem _item;
        
        private void Awake()
        {
            _item = GetComponent<TItem>();
            if (_item == null)
            {
                Debug.LogError($"The component {typeof(TItem)} is missing on GameObject {gameObject.name}.");
            }
        }
        
        private void OnEnable()
        {
            _runtimeSet = ServiceLocator.Main.Get<TRuntimeSet>();
            if (_runtimeSet == null)
            {
                Debug.LogError($"RuntimeSet of type {typeof(TRuntimeSet)} not found in ServiceLocator.");
                return;
            }
            if (_item != null)
            {
                _runtimeSet.Register(_item);
            }
        }
        
        private void OnDisable()
        {
            if (_runtimeSet != null && _item != null)
            {
                _runtimeSet.Unregister(_item);
            }
        }
    }
}