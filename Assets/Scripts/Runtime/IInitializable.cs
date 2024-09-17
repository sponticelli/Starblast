namespace Starblast
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}