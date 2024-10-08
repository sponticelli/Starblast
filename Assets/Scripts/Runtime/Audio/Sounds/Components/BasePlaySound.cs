using Starblast.Services;
using UnityEngine;

namespace Starblast.Audio
{
    public abstract class BasePlaySound : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField][Range(0f, 1f)] protected float volume = 1f;
        
        [Header("Pitch Range")]
        [SerializeField] protected float minPitch = 1f;
        [SerializeField] protected float maxPitch = 1f;
        
        
        protected ISoundManager soundManager;
        
        public abstract void Play(bool origin = true);
        
        protected virtual void Start()
        {
            soundManager = ServiceLocator.Main.SoundManager;
        }
        
        protected float GetPitch()
        {
            return Random.Range(minPitch, maxPitch);
        }
    }
}