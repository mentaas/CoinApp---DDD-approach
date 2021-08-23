using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.DTOs
{
    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; }
        public string Token { get; set; }
    }
}
