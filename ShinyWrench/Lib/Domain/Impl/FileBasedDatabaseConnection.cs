using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;
using WebApplication1.Lib.Domain.Models;
using System.Linq;

namespace WebApplication1.Lib.Domain.Impl
{
    public class FileBasedDatabaseConnection : IDatabaseConnection
    {
        private readonly string dbPath;

        public FileBasedDatabaseConnection(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public void Apply(IModel data)
        {
            var serializedObject = data.Serialize();
            lock (dbPath) { File.AppendAllText(dbPath, string.Format("{0}\n", serializedObject)); }
        }

        public T[] Get<T>(Func<T, bool> filter) where T: IModel
        {
            string[] objectList;
            var matchList = new List<T>();
            lock (dbPath) { objectList = File.ReadAllLines(dbPath); }

            return objectList.Select(x => x.Deserialize<T>()).Where(x => filter(x)).ToArray();
        }
    }
}