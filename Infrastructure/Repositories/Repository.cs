namespace swishes.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Data;
    using System.Data.Entity;

    using swishes.Infrastructure.DataAccess;

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDataContext _context;
        private readonly IDbSet<T> _dbset;

        public Repository(IDataContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbset;
        }

        public virtual RepositoryOperationStatus Add(T entity)
        {
            if (_dbset.Contains(entity))
            {
                return RepositoryOperationStatus.EntityAlreadyExists;
            }
            if (_dbset.Add(entity) != null)
            {
                return RepositoryOperationStatus.Success;
            }
            return RepositoryOperationStatus.UnknownError;
        }

        public virtual RepositoryOperationStatus Delete(T entity)
        {
            if (!_dbset.Contains(entity))
            {
                return RepositoryOperationStatus.EntityNotFound;
            }
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            if (_dbset.Remove(entity) != null)
            {
                return RepositoryOperationStatus.Success;
            }
            return RepositoryOperationStatus.UnknownError;
        }

        public virtual RepositoryOperationStatus DeleteAll(IEnumerable<T> entities)
        {
            RepositoryOperationStatus result = RepositoryOperationStatus.Success;
            foreach (var entity in entities)
            {
                var singleResult = Delete(entity);
                if (singleResult != RepositoryOperationStatus.Success
                    || result != RepositoryOperationStatus.Success)
                {
                    result = RepositoryOperationStatus.NotAllEntitiesProcessed;
                }
            }
            return result;
        }

        public virtual RepositoryOperationStatus Update(T entity)
        {
            if (!_dbset.Contains(entity))
            {
                return RepositoryOperationStatus.EntityNotFound;
            }
            var entry = _context.Entry(entity);
            if (_dbset.Attach(entity) != null)
            {
                entry.State = EntityState.Modified;
            }
            return RepositoryOperationStatus.UnknownError;
        }

        public virtual bool Any()
        {
            if (_dbset != null)
            {
                return _dbset.Any();
            }
            return false;
        }
    }
}