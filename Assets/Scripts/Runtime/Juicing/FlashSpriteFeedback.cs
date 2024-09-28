using System.Collections;
using UnityEngine;

namespace Starblast.Juicing
{
    public class FlashSpriteFeedback : Feedback
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [Header("Settings")]
        [SerializeField] private float _flashDuration = 0.1f;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private Color _defaultColor = Color.white;
        public override void CreateFeedback()
        {
            StartCoroutine(FlashSpriteCoroutine());
        }

        public override void CompletePreviousFeedback()
        {
            StopAllCoroutines();
            _spriteRenderer.color = _defaultColor;
        }
        
        private IEnumerator FlashSpriteCoroutine()
        {
            _spriteRenderer.color = _flashColor;
            yield return new WaitForSeconds(_flashDuration);
            _spriteRenderer.color = _defaultColor;
        }
    }
}