namespace Starblast.Environments.Boundaries
{
    public interface IBoundaryVisualEffectController
    {
        void ResetEffects();
        void ApplyWarningEffects();
        void ApplyDangerEffects();
        void ApplyDeadEffects();
    }
}