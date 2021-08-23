using CoinApp.API.DTOs;
using CoinApp.API.Extentions;
using CoinApp.API.Helpers;
using CoinApp.API.Interfaces;
using CoinApp.Domain.Entities;
using CoinApp.Infrastructure.JwtAuthentication.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly Token _token;

        public AuthController(IUserService userService,
                              IJwtTokenGenerator jwtTokenGenerator,
                              IOptions<Token> token)
        {
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _token = token.Value;
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Generate new token", Description = "If the token is expired you can generate new valid token with expired token and refreshToken")]
        [ProducesResponseType(200)]
        public IActionResult Refresh([FromBody] RefreshTokenDTO refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(refreshToken.Token);

                var username = principal.Identity.Name;
                var savedRefreshToken = "";
                savedRefreshToken = _userService.GetUserRefreshToken(username); //retrieve the refresh token from a data store
                if (savedRefreshToken != refreshToken.RefreshToken)
                    return Unauthorized();

                var newJwtToken = _jwtTokenGenerator.GenerateAccessTokenWithClaimsPrincipal(username, principal.Claims);
                var newRefreshToken = RefreshTokenHelper.GenerateRefreshToken();
                _userService.SaveUserRefreshToken(username, newRefreshToken);


                return Ok(new
                {
                    RefreshToken = newRefreshToken,
                    Token = newJwtToken.AccessToken,
                    ValidTokenTimeInMinutes = _token.ValidTimeInMinutes,
                    ValidDateTimeToken = DateTime.Now.AddMinutes(_token.ValidTimeInMinutes)
                });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized();
            }

        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.SigningKey)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
