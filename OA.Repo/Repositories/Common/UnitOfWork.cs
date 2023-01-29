using Microsoft.EntityFrameworkCore;
using OA.Data;
using OA.Repo.Contracts;
using OA.Repo.Contracts.Common;
using System;

namespace OA.Repo.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public DbContext Context
        {
            get { return _context; }
        }

        public IAuthenticationRepository AuthenticationRepository => throw new NotImplementedException();

        public UnitOfWork(MyboogydbContext appDbContext)
        {
            this._context = appDbContext;
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public DbSet<T> DbSet<T>() where T : class
        {
            return _context.Set<T>();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        public string GetDbConnectionString()
        {
            return _context.Database.GetConnectionString();
        }

        public void SetNewValues<T>(T original, T modified) where T : class
        {
            _context.Entry<T>(original).CurrentValues.SetValues(modified);
        }
    }
}
