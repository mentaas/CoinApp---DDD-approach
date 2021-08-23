using CoinApp.API.DTOs;
using CoinApp.API.Extentions;
using CoinApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create User", Description = "Create User. This endpoint to create system user.")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        public IActionResult Create([FromBody] CreateUserDTO _) => _service.Create(_);

        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login to system", Description = "User have to be auhtorize in order to use authorized endpoints of this API.")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public IActionResult Login([FromBody] Credential credentials) => _service.Login(credentials);


    }
}
