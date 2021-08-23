using CoinApp.Domain.Entities;
using CoinApp.Domain.Repositories;
using CoinApp.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.Infrastructure.Repository
{
    public class CoinRepository : ICoinRepository
    {
        private readonly IRepository<Coin> _repository;

        public CoinRepository(IRepository<Coin> repository)
        {
            _repository = repository;
        }

        public void Save(Coin _) => _repository.Add(_);
        public void UpdateCoin(Coin _) => _repository.Update(_);
        public void DeleteCoin(Coin _) => _repository.Delete(_);
        public Coin GetCoinById(string id) => _repository.GetSingleByCriteria(t => t.Id.ToLower().Equals(id.ToLower()));
        public Coin GetCoinBySymbol(string symbol) => _repository.GetSingleByCriteria(t => t.Symbol == symbol);
        public IEnumerable<Coin> GetCoins() => _repository.ListAll().OrderByDescending(d => d.Rank);
        public IEnumerable<Coin> GetCoinsWithCriteria(Expression<Func<Coin, bool>> criteria) => _repository.ListByCriteria(criteria).OrderByDescending(d => d.Id);
    }
}
