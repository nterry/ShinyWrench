using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace WebApplication1.Lib.Domain.Impl
{
    public class FileBasedDatabaseConnection : IDatabaseConnection
    {
        private readonly string dbPath;

        public FileBasedDatabaseConnection(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public void Apply(object data)
        {
            var serializedObject = JsonConvert.SerializeObject(data);
            lock (dbPath) { File.AppendAllText(dbPath, string.Format("{0}\n", serializedObject)); }
        }

        public T[] Get<T>(Dictionary<string, string> searchFields, Func<string[]> matchSpecifier = null)
        {
            string[] objectList;
            var matchList = new List<T>();
            lock (dbPath) { objectList = File.ReadAllLines(dbPath); }
            objectList.ForEach(s =>
            {
                var notMatch = false;
                searchFields.ForEach(x =>
                {
                    var searchPattern = string.Format("\"{0}\"\\s*:\\s*\"{1}\"", x.Key, x.Value);
                    var r = new Regex(searchPattern, RegexOptions.IgnoreCase);
                    if (!r.IsMatch(s)) notMatch = true; //TODO: Need to find a way to optimize this.
                });

                if (!notMatch) matchList.Add((T) JsonConvert.DeserializeObject(s, typeof (T)));
            });

            return matchList.ToArray();
        }
    }
}