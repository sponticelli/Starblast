using Starblast.Environments;
using Starblast.Services;
using UnityEngine;

namespace Starblast.Juicing
{
    public class PlayFXFeedback : Feedback
    {
        [SerializeField] private FXTypes _fxType;
        [SerializeField] private bool _oppositeDirection;
        public override void CreateFeedback()
        {
            CompletePreviousFeedback();
            
            FXFactory _fxManager = ServiceLocator.Main.Get<FXFactory>();
            
            // if _oppositeDirection is true, the rotation will be transformed to face the opposite direction
            Quaternion rotation = _oppositeDirection ? Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180) : transform.rotation;
            
            _fxManager.Spawn(_fxType, transform.position, rotation);
        }

        public override void CompletePreviousFeedback()
        {
            // Do nothing
        }
    }
}