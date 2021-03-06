using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinApp.Infrastructure.JwtAuthentication.Abstractions;

namespace CoinApp.Infrastructure.JwtAuthentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add cookie backed JWT authentication to the application using the passed in validation parameters
        /// </summary>
        /// <param name="services"></param>
        /// <param name="tokenValidationParams"></param>
        /// <param name="applicationDiscriminator">
        /// An optional unique string that identifies the application to the data protection api for encryption key isolation.
        /// </param>
        /// <param name="authUrlOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationWithProtectedCookie(this IServiceCollection services,
            TokenValidationParameters tokenValidationParams,
            string applicationDiscriminator = null,
            AuthUrlOptions authUrlOptions = null)
        {
            if (tokenValidationParams == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(tokenValidationParams)} is a required parameter. " +
                    $"Please make sure you've provided a valid instance with the appropriate values configured.");
            }

            var hostingEnvironment = services.BuildServiceProvider().GetService<IWebHostEnvironment>();
            // The JwtAuthTicketFormat representing the cookie needs an IDataProtector and
            // IDataSerialiser to correctly encrypt/decrypt and serialise/deserialise the payload
            // respectively. This requirement is enforced by ISecureDataFormat interface in ASP.NET
            // Core. Read more about ASP.NET Core Data Protection API here:
            // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/
            // NB: This is only required if you're using JWT with Cookie based authentication, for
            //     cookieless auth (such as with a Web API) the data protection and serialisation
            //     dependencies won't be needed. You simply need to set the validation params and add
            //     the token generator dependencies and use the right authentication extension below.
            services.AddDataProtection(options => options.ApplicationDiscriminator =
                $"{applicationDiscriminator ?? hostingEnvironment.ApplicationName}")
                .SetApplicationName($"{applicationDiscriminator ?? hostingEnvironment.ApplicationName}");

            services.AddScoped<IDataSerializer<AuthenticationTicket>, TicketSerializer>();

            //cookie expiration should be set the same as the token expiry
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(serviceProvider =>
                new JwtTokenGenerator(tokenValidationParams.ToTokenOptions(43200)));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = hostingEnvironment.ApplicationName;
                // cookie expiration should be set the same as the token expiry (the default is 5
                // mins). The token generator doesn't provide auto-refresh of an expired token so the
                // user will be logged out the next time they try to access a secured endpoint. They
                // will simply have to re-login and acquire a new token and by extension a new cookie.
                // Perhaps in the future I can add some kind of hooks in the token generator that can
                // let the referencing application know that the token has expired and the developer
                // can then request a new token without the user having to re-login.
                options.Cookie.Expiration = TimeSpan.FromDays(30);
                options.Cookie.IsEssential = true;
                // Specify the TicketDataFormat to use to validate/create the ASP.NET authentication
                // ticket. Its important that the same validation parameters are passed to this class
                // so that the token validation works correctly. The framework will call the
                // appropriate methods in JwtAuthTicketFormat based on whether the cookie is being
                // sent out or coming in from a previously authenticated user. Please bear in mind
                // that if the incoming token is invalid (may be it was tampered or spoofed) the
                // Unprotect() method in JwtAuthTicketFormat will simply return null and the
                // authentication will fail.
                options.TicketDataFormat = new JwtAuthTicketFormat(tokenValidationParams,
                    services.BuildServiceProvider().GetService<IDataSerializer<AuthenticationTicket>>(),
                    services.BuildServiceProvider().GetDataProtector(new[]
                    {
                        $"{applicationDiscriminator ?? hostingEnvironment.ApplicationName}-Auth1"
                    }));

                options.LoginPath = authUrlOptions != null ?
                    new PathString(authUrlOptions.LoginPath)
                    : new PathString("/Account/Login");
                options.LogoutPath = authUrlOptions != null ?
                    new PathString(authUrlOptions.LogoutPath)
                    : new PathString("/Account/Logout");
                options.AccessDeniedPath = authUrlOptions != null ?
                   new PathString(authUrlOptions.DeniedPath)
                   : options.LoginPath;
                options.ReturnUrlParameter = authUrlOptions?.ReturnUrlParameter ?? "returnUrl";
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthenticationForAPI(this IServiceCollection services,
                                                                    TokenValidationParameters tokenValidationParams,
                                                                    int validTimeInMinutes)
        {
            if (tokenValidationParams == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(tokenValidationParams)} is a required parameter. " +
                    $"Please make sure you've provided a valid instance with the appropriate values configured.");
            }

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(serviceProvider =>
                new JwtTokenGenerator(tokenValidationParams.ToTokenOptions(validTimeInMinutes)));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = false;
                options.TokenValidationParameters = tokenValidationParams;
            });

            return services;
        }
    }

    /// <summary>
    /// A simple structure to store the configured login/logout paths and the name of the return url parameter
    /// </summary>
    public sealed class AuthUrlOptions
    {
        /// <summary>
        /// The login path to redirect the user to incase of unauthenticated requests.
        /// Default is "/Account/Login"
        /// </summary>
        public string LoginPath { get; set; }

        /// <summary>
        /// The path to redirect the user to once they have logged out.
        /// Default is "/Account/Logout"
        /// </summary>
        public string LogoutPath { get; set; }

        /// <summary>
        /// The path to redirect the user to following a successful authentication attempt.
        /// Default is "returnUrl"
        /// </summary>
        public string ReturnUrlParameter { get; set; }



        public string DeniedPath { get; set; }
    }
}
