using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinApp.API.Interfaces
{
    public interface ICoinService
    {
        Task<ObjectResult> GetCoins();
        Task<ObjectResult> GetFavoriteCoins();

        Task<ObjectResult> SaveRemoveCoinAsFavorite(string id);
    }
}
