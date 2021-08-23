using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoinApp.API.Helpers
{
    public static class ApiResponseHelper
    {
        public static ObjectResult Response(HttpStatusCode statusCode, string message)
        {
            var response = new ObjectResult(message);
            response.StatusCode = (int)statusCode;
            return response;

        }

        public static ObjectResult ResponseOk(object result)
        {
            var response = new ObjectResult(result);
            response.StatusCode = (int)HttpStatusCode.OK;
            return new ObjectResult(result);
        }
    }
}
