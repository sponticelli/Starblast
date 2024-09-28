using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Starblast.Juicing
{
    public class FlashLight2DFeedback : Feedback
    {
        [Header("References")] 
        [SerializeField] private Light2D _lightTarget;

        [Header("Settings")] [SerializeField] private float _lightOnDelay = 0.01f;
        [SerializeField] private float _lightOffDelay = 0.01f;
        [SerializeField] private bool _defaultState = false;

        public override void CreateFeedback()
        {
            StartCoroutine(ToggleLightCoroutine(_lightOnDelay, true,
                () => { StartCoroutine(ToggleLightCoroutine(_lightOffDelay, false)); }));
        }

        public override void CompletePreviousFeedback()
        {
            StopAllCoroutines();
            _lightTarget.enabled = _defaultState;
        }

        private IEnumerator ToggleLightCoroutine(float time, bool state, Action onComplete = null)
        {
            yield return new WaitForSeconds(time);
            _lightTarget.enabled = state;
            onComplete?.Invoke();
        }

    }
}