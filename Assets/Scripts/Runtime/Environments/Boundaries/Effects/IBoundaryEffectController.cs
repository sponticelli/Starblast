namespace Starblast.Environments.Boundaries
{
    public interface IBoundaryEffectController
    {
        void ResetEffects();
        void OnEnterZone(ZoneType zone);
        void OnExitZone(ZoneType zone);
        void SetEffectIntensity(float intensity);
    }
}