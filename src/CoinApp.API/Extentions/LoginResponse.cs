using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Extentions
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string FirstLast { get; set; }
        public string EmailAddress { get; set; }
        public int ValidTokenTimeInMinutes { get; set; }
        public DateTime ValidDateTimeToken { get; set; }
    }
}
