using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CoinApp.Domain.SeedWork
{
    public interface IRepository<T> //where T : Entity
    {
        //T GetById(int id, params string[] includes);
        IEnumerable<T> ListAll(params string[] includes);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        List<T> ListByCriteria(Expression<Func<T, bool>> criteria, params string[] includes);

        T GetSingleByCriteria(Expression<Func<T, bool>> criteria);
        T GetSingleByCriteria(Expression<Func<T, bool>> criteria, params string[] includes);
        bool Any(Expression<Func<T, bool>> criteria);

        //T GetLastEntity();

        void AddRange(IEnumerable<T> entity);
        void Save();

    }
}
