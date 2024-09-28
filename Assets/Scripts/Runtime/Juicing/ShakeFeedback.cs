using DG.Tweening;
using UnityEngine;

namespace Starblast.Juicing
{
    public class ShakeFeedback : Feedback
    {
        [Header("References")]
        [SerializeField] private GameObject _objectToShake;
        
        [Header("Settings")]
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _strength = 0.5f;
        [SerializeField] private int _vibrato = 10;
        [SerializeField] private float _randomness = 90;
        [SerializeField] private bool _snapping = false;
        [SerializeField] private bool _fadeOut = true;
        public override void CreateFeedback()
        {
            CompletePreviousFeedback();
            _objectToShake.transform.DOShakePosition(_duration, _strength, _vibrato, _randomness, 
                _snapping, _fadeOut);
        }

        public override void CompletePreviousFeedback()
        {
            _objectToShake.transform.DOComplete();
        }
    }
}