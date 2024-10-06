using UnityEngine;

namespace Starblast.Audio
{
    public class PlaySoundClip : BasePlaySound
    {
        [Header("Clip")]
        [SerializeField] private AudioClip clip;
        
        public override void Play(bool origin = true)
        {
            soundManager.Play(clip, position: origin ? Vector3.zero : transform.position, 
                pitch: GetPitch(), volume: volume);
        }
    }
}