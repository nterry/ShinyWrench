namespace WebApplication1.Lib.Domain
{
    public interface IDatabaseConnector
    {
        IDatabaseConnection Connect(string url, string username, string password, string dbName);
    }
}
