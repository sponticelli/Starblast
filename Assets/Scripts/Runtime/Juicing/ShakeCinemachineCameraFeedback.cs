using System.Collections;
using UnityEngine;

namespace Starblast.Juicing
{
    public class ShakeCinemachineCameraFeedback : Feedback
    {
        [Header("References")]
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;
        
        [Header("Settings")]
        [SerializeField][Range(0,5f)] private float _amplitude = 1.0f;
        [SerializeField][Range(0,5f)] private float _intensity = 1.0f;
        [SerializeField][Range(0,1f)] private float _duration = 0.1f;
        
        private Cinemachine.CinemachineBasicMultiChannelPerlin _noise;
        
        private void Start()
        {
            if (_camera == null)
            {
                _camera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            }
            _noise = _camera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }
        
        public override void CreateFeedback()
        {
            CompletePreviousFeedback();
            _noise.m_AmplitudeGain = _amplitude;
            _noise.m_FrequencyGain = _intensity;
            StartCoroutine(ShakeCoroutine());
        }

        public override void CompletePreviousFeedback()
        {
            StopAllCoroutines();
            _noise.m_AmplitudeGain = 0;
        }

        private IEnumerator ShakeCoroutine()
        {
            for (var i = _duration; i > 0; i -= Time.deltaTime)
            {
                _noise.m_AmplitudeGain = Mathf.Lerp(0, _amplitude, i / _duration);
                yield return null;
            }
            _noise.m_AmplitudeGain = 0;
        }
    }
}