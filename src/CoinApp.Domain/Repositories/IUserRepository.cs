using CoinApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CoinApp.Domain.Repositories
{
    public interface IUserRepository
    {
        void Save(User _);
        void UpdateUser(User _);
        void DeleteUser(User _);
        bool CheckUserEmail(string emailAddress, int? IdUser);
        User GetUserById(int id);
        User GetUserByIdWithInclude(int id);
        User GetUserByEmail(string email);
        IEnumerable<User> GetUsers();
        IEnumerable<User> GetUsersWithCriteria(Expression<Func<User, bool>> criteria);
    }
}
