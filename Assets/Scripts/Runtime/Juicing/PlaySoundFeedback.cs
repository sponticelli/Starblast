using Starblast.Audio;
using UnityEngine;

namespace Starblast.Juicing
{
    public class PlaySoundFeedback : Feedback
    {
        [Header("References")]
        [SerializeField] private BasePlaySound _sound;
        
        public override void CreateFeedback()
        {
            _sound.Play();
        }

        public override void CompletePreviousFeedback()
        {
            // Do nothing
        }
    }
}