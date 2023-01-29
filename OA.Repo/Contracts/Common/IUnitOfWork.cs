using Microsoft.EntityFrameworkCore;
using OA.Repo.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Repo.Contracts.Common
{
    public interface IUnitOfWork : IDisposable
    {
        int Complete();
        string GetDbConnectionString();
        DbSet<T> DbSet<T>() where T : class;
        void SetNewValues<T>(T original, T modified) where T : class;

    }
}
