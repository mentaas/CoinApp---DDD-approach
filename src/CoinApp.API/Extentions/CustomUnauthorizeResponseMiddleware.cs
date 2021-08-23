using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Extentions
{
    public class CustomUnauthorizeResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomUnauthorizeResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                var result = JsonConvert.SerializeObject(new { status = 4 });//(int)PublicResultStatusCodes.NotAuthorized });
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            //else
            //{
            //    await _next(context);
            //}
        }
    }
}
