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
    public class UserRepository : IUserRepository
    {
        private readonly IRepository<User> _userRepository;

        public UserRepository(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public void Save(User _) => _userRepository.Add(_);
        public void UpdateUser(User _) => _userRepository.Update(_);
        public void DeleteUser(User _) => _userRepository.Update(_);
        public bool CheckUserEmail(string emailAddress, int? IdUser)
        {
            if (IdUser == null)
                return _userRepository.Any(t => t.EmailAddress == emailAddress);
            else
                return _userRepository.Any(t => t.EmailAddress == emailAddress && t.Id != IdUser);
        }
        public User GetUserById(int id) => _userRepository.GetSingleByCriteria(t => t.Id == id);
        public User GetUserByIdWithInclude(int id) => _userRepository.GetSingleByCriteria(t => t.Id == id, "Coins");
        public User GetUserByEmail(string email) => _userRepository.GetSingleByCriteria(t => t.EmailAddress == email);

        public IEnumerable<User> GetUsers() => _userRepository.ListAll().OrderByDescending(d => d.Id);
        public IEnumerable<User> GetUsersWithCriteria(Expression<Func<User, bool>> criteria) => _userRepository.ListByCriteria(criteria).OrderByDescending(d => d.Id);

    }
}
