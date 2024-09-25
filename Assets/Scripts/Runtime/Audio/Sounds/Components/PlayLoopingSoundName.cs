using UnityEngine;

namespace Starblast.Audio
{
    public class PlayLoopingSoundName : APlayLoopingSound
    {
        [Header("Sound Name")]
        [SerializeField] private SoundNameEnum soundName;
        
        protected override void InternalPlay(bool origin = true)
        {
            soundId = soundManager.PlayLoop(soundName, position: origin ? Vector3.zero : transform.position, 
                pitch: GetPitch(), volume: volume);
        }
    }
}