using CoinApp.API.DTOs;
using CoinApp.API.Extentions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Interfaces
{
    public interface IUserService
    {
        ObjectResult Create(CreateUserDTO _);
        ObjectResult Login(Credential credentials);

        string GetUserRefreshToken(string email);
        void SaveUserRefreshToken(string email, string newRefreshToken);

    }
}
