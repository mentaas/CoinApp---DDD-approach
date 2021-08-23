using AutoMapper;
using CoinApp.API.DTOs;
using CoinApp.API.Extentions;
using CoinApp.API.Helpers;
using CoinApp.API.Interfaces;
using CoinApp.Domain.Entities;
using CoinApp.Domain.Repositories;
using CoinApp.Infrastructure.JwtAuthentication.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoinApp.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly Token _token;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserService(IUserRepository repository,
                           IJwtTokenGenerator jwtTokenGenerator,
                           IMapper mapper,
                           IOptions<Token> token,
                           IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _token = token.Value;
            _contextAccessor = contextAccessor;
        }

        public ObjectResult Create(CreateUserDTO _)
        {
            try
            {
                var newUser = _repository.GetUserByEmail(_.EmailAddress);

                if (newUser != null)
                    return ApiResponseHelper.Response(HttpStatusCode.BadRequest, "Account already exist! Email :" + _.EmailAddress);
                

                var user = _mapper.Map<User>(_);
                HashNewUserPassword(ref user);


                _repository.Save(user);

                return ApiResponseHelper.ResponseOk(_mapper.Map<UserDTO>(user));

            }
            catch (Exception ex) { return ApiResponseHelper.Response(HttpStatusCode.InternalServerError, "Internal Server Error"); }
        }

        private void HashNewUserPassword(ref User user)
        {
            user.EntryDate = DateTime.Now;
            user.RefreshToken = RefreshTokenHelper.GenerateRefreshToken();
            var hashHelper = new HashHelper(user.Password);
            user.Password = hashHelper.Hash;
            user.PasswordSalt = hashHelper.Salt;
        }

        public ObjectResult Login(Credential credentials)
        {
            var user = _repository.GetUserByEmail(credentials.Email);
            var validUsername = false;

            if (user != null)
                validUsername = true;


            if (validUsername)
            {

                if (IsValidUser(credentials.Email, credentials.Password))
                {
                    _contextAccessor.HttpContext = IdentityHelper.SetIdentity(_contextAccessor.HttpContext, user);
                    var accessTokenResult = _jwtTokenGenerator.GenerateAccessTokenWithClaimsPrincipal(credentials.Email, AddMyClaims(user));

                    var response = new LoginResponse
                    {
                        Id = user.Id,
                        Token = accessTokenResult.AccessToken,
                        RefreshToken = user.RefreshToken,
                        ValidTokenTimeInMinutes = _token.ValidTimeInMinutes,
                        ValidDateTimeToken = DateTime.Now.AddMinutes(_token.ValidTimeInMinutes),
                        FirstLast = user.FirstName + " " + user.LastName,
                        EmailAddress = user.EmailAddress
                    };

                    return ApiResponseHelper.ResponseOk(response);
                }
            }
            return ApiResponseHelper.Response(HttpStatusCode.BadRequest, "The credentials are not valid!");
        }

        public bool IsValidUser(string email, string password)
        {
            var user = _repository.GetUserByEmail(email);

            if (user == null)
                return false;

            return HashHelper.Verify(user.PasswordSalt, user.Password, password);
        }
        private static IEnumerable<Claim> AddMyClaims(User user)
        {
            var myClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };

            return myClaims;
        }

        public string GetUserRefreshToken(string email) => _repository.GetUserByEmail(email).RefreshToken;

        public void SaveUserRefreshToken(string email, string newRefreshToken)
        {
            var user = _repository.GetUserByEmail(email);
            user.RefreshToken = newRefreshToken;

            _repository.UpdateUser(user);
        }
    }
}
