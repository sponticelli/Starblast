namespace Starblast.Data
{
    public interface IDataProvider<out T> where T : IData
    {
        T Data { get; }
    }
}