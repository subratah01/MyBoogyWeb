using Microsoft.EntityFrameworkCore;
using OA.Data;
using OA.Repo.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OA.Repo.Repositories.Common
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly MyboogydbContext context;        
        internal DbSet<T> Entities { get; private set; }
        public RepositoryBase(MyboogydbContext context)
        {
            this.context =  context;
            Entities = context.Set<T>();

        }
        public IEnumerable<T> Get(Expression<Func<T, bool>> predict)
        {
            IEnumerable<T> result = null;
            if (predict!=null)
            {
                result= Entities.Where(predict);
            }
            return result;
        }
        public IEnumerable<T> GetAll()
        {
            return Entities.AsEnumerable();
        }
        public T GetFirst()
        {
            return Entities.First();
        }
        public T GetFirst(Expression<Func<T, bool>> predict)
        {
            return Entities.First(predict);
        }
        public T GetFirstOrDefault()
        {
            return Entities.FirstOrDefault();
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> predict)
        {
            return GetFirstOrDefault(predict, false);
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> predict, bool local)
        {
            T result = default(T);
            if (predict!=null)
            {
                result = local ? Entities.Local.FirstOrDefault(predict.Compile()) : Entities.FirstOrDefault(predict);
            }
            return result;
        }
        public T GetSingle()
        {
            return Entities.Single();
        }
        public T GetSingle(Expression<Func<T, bool>> predict)
        {
            return Entities.Single(predict);
        }
        public T GetSingleOrDefault()
        {
            return Entities.SingleOrDefault();
        }
        public T GetSingleOrDefault(Expression<Func<T, bool>> predict)
        {
            return GetSingleOrDefault(predict);
        }
        public IQueryable<T> GetAsQueryable(Expression<Func<T, bool>> predict)
        {
            IQueryable<T> result = Entities;
            if (predict!=null)
            {
                result = Entities.Where(predict);
            }
            return result;
        }
        public IQueryable<T> GetAllAsQueryable(Expression<Func<T, bool>> predict)
        {
            return Entities;
        }
        public bool Exists(Expression<Func<T, bool>> predict)
        {
            return predict == null
                ? Entities.Any()
                : Entities.Any(predict);
        }
        public long Count()
        {
            return Entities.Count();
        }
        public long Count(Expression<Func<T, bool>> predict)
        {
            return Entities.Count(predict);
        }
        public T Add(T entity)
        {
            if (entity==null)
            {
                throw new ArgumentNullException("instance");
            }

            T return_entity= Entities.Add(entity).Entity;
            context.SaveChanges();
            return return_entity;
        }
        public void Delete(Expression<Func<T, bool>> predict)
        {
            T entity = GetFirstOrDefault(predict);
            if (entity!=null)
            {
                Entities.Remove(entity);
            }
            context.SaveChanges();
        }
        public void DeleteAll(Expression<Func<T, bool>> predict)
        {
            var items = Get(predict).ToList();
            foreach (var item in items)
            {
                if (item != null)
                {
                    Entities.Remove(item);
                }
            }
            context.SaveChanges();
        }
        public void Update(T entity)
        {
            Entities.Update(entity);
            context.SaveChanges();
        }

        
    }
}
