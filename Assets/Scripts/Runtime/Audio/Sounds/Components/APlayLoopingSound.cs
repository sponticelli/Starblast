using UnityEngine;

namespace Starblast.Audio
{
    public abstract class APlayLoopingSound : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField][Range(0f, 1f)] protected float volume = 1f;
        
        [Header("Pitch Range")]
        [SerializeField] protected float minPitch = 1f;
        [SerializeField] protected float maxPitch = 1f;
        
        protected ISoundManager soundManager;
        protected int soundId;
        
        protected virtual void Start()
        {
            soundManager = ServiceLocator.Main.SoundManager;
        }

        public void Play(bool origin = true)
        {
            Stop();
            InternalPlay(origin);
        }
        
        protected abstract void InternalPlay(bool origin = true);

        public void Stop()
        {
            if (soundId != 0)
            {
                soundManager.StopLoop(soundId);
                soundId = 0;
            }
        }
        
        protected float GetPitch()
        {
            return Random.Range(minPitch, maxPitch);
        }
    }
}