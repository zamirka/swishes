namespace swishes.Infrastructure.Repositories
{
    using System;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        int Save();
    }
}