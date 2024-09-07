namespace Starblast.Inputs
{
    public delegate void InputEvent();
    public delegate void InputEvent<in T>(T value);
}