using CoinApp.Domain.SeedWork;
using CoinApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.Infrastructure.Repository
{
    public class EfRepository<T> : IRepository<T> where T : class //where T : class
    {
        protected readonly CoinAppContext _dbContext;

        public EfRepository(CoinAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public virtual T GetById(int id, params string[] includes)
        //{
        //    IQueryable<T> query = _dbContext.Set<T>();

        //    if (includes != null)
        //    {
        //        int count = includes.Length;
        //        for (int index = 0; index < count; index++)
        //        {
        //            query = query.Include(includes[index]);
        //        }
        //    }
        //    return query.FirstOrDefault(t => t.Id == id);
        //}

        public IEnumerable<T> ListAll(params string[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null)
            {
                int count = includes.Length;
                for (int index = 0; index < count; index++)
                {
                    query = query.Include(includes[index]);
                }
            }

            return query.AsNoTracking().AsEnumerable();
        }

        public T GetSingleByCriteria(Expression<Func<T, bool>> criteria)
        {
            return ListByCriteria(criteria).FirstOrDefault();
        }

        public T GetSingleByCriteria(Expression<Func<T, bool>> criteria, params string[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null)
            {
                int count = includes.Length;
                for (int index = 0; index < count; index++)
                {
                    query = query.Include(includes[index]);
                }
            }

            return query.AsNoTracking().Where(criteria).FirstOrDefault();
        }

        public bool Any(Expression<Func<T, bool>> criteria)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            return query.Any(criteria);
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public List<T> ListByCriteria(Expression<Func<T, bool>> criteria, params string[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes != null)
            {
                int count = includes.Length;
                for (int index = 0; index < count; index++)
                {
                    query = query.Include(includes[index]);
                }
            }

            return query.AsNoTracking().Where(criteria).ToList();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        //public virtual T GetLastEntity()
        //{
        //    return _dbContext.Set<T>().OrderByDescending(t => t.Id).FirstOrDefault();
        //}

        public void AddRange(IEnumerable<T> list)
        {
            _dbContext.Set<T>().AddRange(list);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            _dbContext.SaveChanges();
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            _dbContext.SaveChanges();
        }
    }
}
