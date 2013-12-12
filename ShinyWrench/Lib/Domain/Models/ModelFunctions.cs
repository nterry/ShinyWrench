using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Lib.Domain.Models
{
    public static class ModelFunctions
    {
        public static T Deserialize<T>(this string data) where T: IModel
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string Serialize<T>(this T data) where T : IModel
        {
            return JsonConvert.SerializeObject(data);            
        }
    }
}