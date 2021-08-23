using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Extentions
{
    public class Token
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public int ValidTimeInMinutes { get; set; }
    }
}
