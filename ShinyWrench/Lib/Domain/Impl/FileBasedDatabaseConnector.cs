using System.IO;
using WebApplication1.Lib.Domain.Models;

namespace WebApplication1.Lib.Domain.Impl
{
    public class FileBasedDatabaseConnector : IDatabaseConnector
    {
        public IDatabaseConnection Connect(string dbDir, string username, string password, string dbName)
        {
            if (!Directory.Exists(dbDir)) 
                Directory.CreateDirectory(dbDir);

            string dbFile = DbLocation(dbDir, dbName);
            var returnFile = new FileBasedDatabaseConnection(dbFile);
            if (!File.Exists(dbFile))
            {
                // This is just in case the AppendText doesn't automatically open it
                File.Create(dbFile).Close();
                User testUser = new User
                {
                    Username = "Admin",
                    Password = "Changeme"
                };

                returnFile.Apply(testUser);
            }

            return returnFile;
        }

        private string DbLocation(string dir = @"c:\data", string dbFile = "test")
        {
            return string.Format("{0}\\{1}.db", dir, dbFile);
        }

    }
}