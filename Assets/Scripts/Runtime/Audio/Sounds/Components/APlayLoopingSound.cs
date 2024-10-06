using Starblast.Services;
using UnityEngine;

namespace Starblast.Audio
{
    public abstract class APlayLoopingSound : BasePlaySound
    {
        protected int soundId;
        
       

        public override void Play(bool origin = true)
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
    }
}