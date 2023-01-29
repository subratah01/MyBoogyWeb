using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OA.Repo.Contracts.Common
{
    public interface IRepositoryBase<T> where T : class    
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> predict);
        IEnumerable<T> GetAll();
        T GetFirst();
        T GetFirst(Expression<Func<T, bool>> predict);
        T GetFirstOrDefault();
        T GetFirstOrDefault(Expression<Func<T, bool>> predict);
        T GetSingle();
        T GetSingle(Expression<Func<T, bool>> predict);
        T GetSingleOrDefault();
        T GetSingleOrDefault(Expression<Func<T, bool>> predict);
        IQueryable<T> GetAsQueryable(Expression<Func<T, bool>> predict);
        IQueryable<T> GetAllAsQueryable(Expression<Func<T, bool>> predict);
        bool Exists(Expression<Func<T, bool>> predict);
        long Count();
        long Count(Expression<Func<T, bool>> predict);
        T Add(T entity);
        void Delete(Expression<Func<T, bool>> predict);
        void DeleteAll(Expression<Func<T, bool>> predict);
        void Update(T entity);


    }
}
