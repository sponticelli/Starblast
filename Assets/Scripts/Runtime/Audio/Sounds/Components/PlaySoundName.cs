using UnityEngine;

namespace Starblast.Audio
{
    public class PlaySoundName : APlaySound
    {
        [Header("Sound Name")]
        [SerializeField] private SoundNameEnum soundName;
        
        public override void Play(bool origin = true)
        {
            soundManager.Play(soundName, position: origin ? Vector3.zero : transform.position, 
                pitch: GetPitch(), volume: volume);
        }
    }
}