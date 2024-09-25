using UnityEngine;

namespace Starblast.Audio
{
    public class PlayLoopingSoundClip : APlayLoopingSound
    {
        [Header("Clip")]
        [SerializeField] private AudioClip clip;
        
        protected override void InternalPlay(bool origin = true)
        {
            soundId = soundManager.PlayLoop(clip, position: origin ? Vector3.zero : transform.position, 
                pitch: GetPitch(), volume: volume);
        }
    }
}