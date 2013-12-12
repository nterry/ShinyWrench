using System;
using System.Collections.Generic;
using WebApplication1.Lib.Domain.Models;

namespace WebApplication1.Lib.Domain
{
    public interface IDatabaseConnection
    {
        void Apply(IModel data);
        T[] Get<T>(Func<T, bool> filter) where T : IModel;
    }
}
