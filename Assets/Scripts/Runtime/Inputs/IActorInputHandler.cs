namespace Starblast.Inputs
{
    public interface IActorInputHandler
    {
        event InputEvent<float> OnRotate;
        event InputEvent<float> OnThrust;
        event InputEvent OnFirePressed;
        event InputEvent OnFireReleased;
        
        float Rotation { get; }
        float Thrust { get; }
    }
}