namespace Starblast.Environments.Boundaries
{
    public interface IBoundaryAudioEffectController
    {
        void ResetEffects();
        void PlayWarningAudio();
        void PlayDangerAudio();
        void PlayDeadAudio();
    }
}