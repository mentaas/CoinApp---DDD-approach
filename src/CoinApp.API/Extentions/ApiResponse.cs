using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Extentions
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public IList<T> Data { get; set; }
    }
}
