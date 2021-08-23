using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoinApp.Domain.SeedWork;
using CoinApp.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using CoinApp.API.Interfaces;
using CoinApp.API.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CoinApp.Infrastructure;
using CoinApp.Infrastructure.Data;
using CoinApp.Infrastructure.JwtAuthentication.Extensions;
using Microsoft.EntityFrameworkCore;
using CoinApp.API.Extentions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using CoinApp.Domain.Repositories;

namespace CoinApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<CoinAppContext>(c =>
                     c.UseSqlServer(Configuration.GetConnectionString("CoinAppConnection")));

            var validationParams = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,

                ValidateAudience = true,
                ValidAudience = Configuration["Token:Audience"],

                ValidateIssuer = true,
                ValidIssuer = Configuration["Token:Issuer"],

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:SigningKey"])),
                ValidateIssuerSigningKey = true,

                RequireExpirationTime = true,
                ValidateLifetime = true,

            };
            var validTime = int.Parse(Configuration["Token:ValidTimeInMinutes"]);

            services.AddJwtAuthenticationForAPI(validationParams, validTime);

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICoinRepository, CoinRepository>();
            services.AddScoped<ICoinService, CoinService>();

            services.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"));
            services.Configure<Token>(Configuration.GetSection("Token"));

            services.AddControllers();

            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoinAppAPI", Version = "v1" });

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "bearer"
                                }
                            },
                            new string[] {}
                    }
                });
                c.AddFluentValidationRules();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TM API V1");
            });

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature.Error;

                //logger.LogError(exception, "General Error");

                var result = JsonConvert.SerializeObject(new { status = 500 });
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
