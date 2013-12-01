namespace swishes.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using swishes.Infrastructure.DataAccess;
    using swishes.Infrastructure.Logging;

    public class UnitOfWork<TContext> : IUnitOfWork where TContext: IDataContext, new()
    {
        private readonly IDataContext _ctx;
        private Dictionary<Type, object> _repositories;
        private bool _disposed;
        private readonly ILogger _logger;

        public UnitOfWork(ILogger logger)
        {
            _ctx = new TContext();
            _repositories = new Dictionary<Type, object>();
            _disposed = false;
            _logger = logger;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;
            }

            var repository = new Repository<TEntity>(_ctx, _logger);

            _repositories.Add(typeof(TEntity), repository);

            return repository;
        }

        public int Save()
        {
            int entitiesAffectdCount = 0;
            try
            {
                entitiesAffectdCount = _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not save changes to Database", ex);
            }
            return entitiesAffectdCount;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _ctx.Dispose();
                }
                this._disposed = true;
            }
        }
    }
}