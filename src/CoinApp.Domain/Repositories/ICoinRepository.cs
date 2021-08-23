using CoinApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.Domain.Repositories
{
    public interface ICoinRepository
    {
        void Save(Coin _);
        void UpdateCoin(Coin _);
        void DeleteCoin(Coin _);
        Coin GetCoinById(string id);
        Coin GetCoinBySymbol(string symbol);
        IEnumerable<Coin> GetCoins();
        IEnumerable<Coin> GetCoinsWithCriteria(Expression<Func<Coin, bool>> criteria);
    }
}
