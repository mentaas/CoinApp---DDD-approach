using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinApp.Helpers.Authentication.Types;

namespace CoinApp.Infrastructure.JwtAuthentication.Extensions
{
    public static class TokenValidationParametersExtensions
    {
        public static TokenOptions ToTokenOptions(this TokenValidationParameters tokenValidationParameters,
            int tokenExpiryInMinutes = 5)
        {
            return new TokenOptions(tokenValidationParameters.ValidIssuer,
                tokenValidationParameters.ValidAudience,
                tokenValidationParameters.IssuerSigningKey,
                tokenExpiryInMinutes);
        }
    }
}
