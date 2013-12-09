using System;
using System.Collections.Generic;

namespace WebApplication1.Lib.Domain
{
    public interface IDatabaseConnection
    {
        void Apply(object data);
        T[] Get<T>(Dictionary<string, string> searchFields, Func<string[]> matchSpecifier = null);
    }
}
