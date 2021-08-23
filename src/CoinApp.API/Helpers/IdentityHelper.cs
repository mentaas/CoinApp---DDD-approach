using CoinApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CoinApp.API.Helpers
{
    public class IdentityHelper
    {
        public static HttpContext SetIdentity(HttpContext context, User user)
        {
            var claims = new[] {
                                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                    new Claim(ClaimTypes.Name, user.EmailAddress),
                                };
            var identity = new ClaimsIdentity(claims, AuthenticationSchemes.Basic.ToString());
            var userPrincipal = new GenericPrincipal(identity, new[] { "admin" });
            context.User = userPrincipal;

            return context;
        }
    }
}
