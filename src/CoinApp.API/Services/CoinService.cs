using CoinApp.API.Helpers;
using CoinApp.API.Interfaces;
using CoinApp.Domain.Entities;
using CoinApp.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoinApp.API.Services
{
    public class CoinService : ICoinService
    {
        private readonly ICoinRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public CoinService(ICoinRepository repository,
                           IUserRepository userRepository,
                           IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<ObjectResult> GetCoins()
        {
            var result = await ApiRequestHelper<Coin>.Get("assets", null);

            return ApiResponseHelper.ResponseOk(result.Data);
        }

        public async Task<ObjectResult> GetFavoriteCoins()
        {
            //get favCoins from DB
            var user = _userRepository.GetUserByIdWithInclude(int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (!user.Coins.Any())
                return ApiResponseHelper.Response(System.Net.HttpStatusCode.NoContent, "You don't have any favorite coin");

            var favCoins = user.Coins.Select(x => x.Id);
            //try to get data from api
            try
            {
                var result = await ApiRequestHelper<Coin>.Get($"assets?ids={string.Join(",", favCoins)}", null);
                return ApiResponseHelper.ResponseOk(result.Data);

            }
            catch { }

            //if api fails try to get data from database
            return ApiResponseHelper.ResponseOk(user.Coins);
        }

        public async Task<ObjectResult> SaveRemoveCoinAsFavorite(string id)
        {
            try
            {
                var result = await ApiRequestHelper<Coin>.GetSingle($"assets/{id}", null);

                if (result != null && result.SingleData != null)
                {
                    var coin = result.SingleData;
                    var user = _userRepository.GetUserByIdWithInclude(int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
                    var isMark = user.Coins.Any(c => c.Id == id);
                    if (isMark)
                    {
                        var toDelete = user.Coins.FirstOrDefault(c => c.Id == id);
                        _repository.DeleteCoin(toDelete);
                        return ApiResponseHelper.Response(System.Net.HttpStatusCode.NoContent, "Coin is unmarked from Favorite!");
                    }
                    var checkIfcoinExistInDB = _repository.GetCoinById(id);
                    if (checkIfcoinExistInDB == null)
                        _repository.Save(coin);

                    coin.Users.Add(user);
                    _repository.UpdateCoin(coin);
                    return ApiResponseHelper.ResponseOk(coin);
                }
                else
                    return ApiResponseHelper.Response(System.Net.HttpStatusCode.ServiceUnavailable, "Third party service is unavailable, try later!");
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}