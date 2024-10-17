namespace Starblast.Services
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
    
    
}