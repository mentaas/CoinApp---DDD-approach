using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Extentions
{
    public class ApiResponse2<T>
    {
        public int Status { get; set; }
        [JsonProperty("data")]
        public T SingleData { get; set; }
    }
}
