using System.IO;

namespace WebApplication1.Lib.Domain.Impl
{
    public class FileBasedDatabaseConnector : IDatabaseConnector
    {
        private const string defaultDBDirPath = @"C:\data";
        private const string defaultDBFilePath = @"C:\data\test.db";

        public FileBasedDatabaseConnector()
        {
            if (!Directory.Exists(defaultDBDirPath)) Directory.CreateDirectory(defaultDBDirPath);
            if (!File.Exists(defaultDBFilePath)) File.Create(defaultDBFilePath);
        }
        public IDatabaseConnection Connect(string dbDir, string username, string password, string dbName)
        {
            if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);
            if (!Directory.Exists(dbName)) Directory.CreateDirectory(dbName);
            return new FileBasedDatabaseConnection(string.Format("{0}\\{1}.db", dbDir, dbName));
        }
    }
}