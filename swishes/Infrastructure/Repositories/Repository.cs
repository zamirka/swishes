namespace swishes.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Data;
    using System.Data.Entity;

    using swishes.Infrastructure.DataAccess;
    using swishes.Infrastructure.Logging;

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDataContext _context;
        private readonly IDbSet<T> _dbset;
        private readonly ILogger _logger;

        public Repository(IDataContext context, ILogger logger)
        {
            _context = context;
            _dbset = context.Set<T>();
            _logger = logger;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbset;
        }

        public virtual RepositoryOperationResult Add(T entity)
        {
            if (entity == null)
            {
                return RepositoryOperationResult.NullEntity;
            }
            int countBefore;
            int countAfter;
            try
            {
                countBefore = _dbset.Count();
                if (_dbset.Add(entity) == null)
                {
                    throw new Exception("Add operation result is null");
                }
                countAfter = _dbset.Count();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("Error occured in Repository<{0}>.Add(entity)", typeof(T).ToString()), ex);
                return RepositoryOperationResult.UnknownError;
            }
            return countAfter > countBefore ? RepositoryOperationResult.Success : RepositoryOperationResult.EntityAlreadyExists;
        }

        public virtual RepositoryOperationResult Delete(T entity)
        {
            if (entity == null)
            {
                return RepositoryOperationResult.NullEntity;
            }
            try
            {
                if (_dbset.Remove(entity) != null)
                {
                    if (_context.SetEntryState(entity, EntityState.Deleted))
                    {
                        return RepositoryOperationResult.Success;
                    }
                    return RepositoryOperationResult.EntityNotFound;
                }
                else
                {
                    throw new Exception("Delete Operation result is null");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("Error occured in Repository<{0}>.Delete(entity)", typeof(T).ToString()), ex);
                return RepositoryOperationResult.UnknownError;
            }
        }

        public virtual RepositoryOperationResult DeleteAll(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                return RepositoryOperationResult.NullEntity;
            }
            RepositoryOperationResult result = RepositoryOperationResult.Success;
            var itemsToDelete = entities.Count();
            for (int i = itemsToDelete - 1; i >= 0; i--)
            {
                var entity = entities.ToArray()[i];
                var singleResult = Delete(entity);
                if (singleResult != RepositoryOperationResult.Success
                    || result != RepositoryOperationResult.Success)
                {
                    result = RepositoryOperationResult.NotAllEntitiesProcessed;
                }
            }
            return result;
        }

        public virtual RepositoryOperationResult Update(T entity)
        {
            if (entity == null)
            {
                return RepositoryOperationResult.NullEntity;
            }
            try
            {
                if (_dbset.Attach(entity) != null)
                {
                    if (_context.SetEntryState(entity, EntityState.Modified))
                    {
                        return RepositoryOperationResult.Success;
                    }
                    return RepositoryOperationResult.EntityNotFound;
                }
                else
                {
                    throw new Exception("Update operation resulted in Error");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("Error occured in Repository<{0}>.Delete(entity)", typeof(T).ToString()), ex);
                return RepositoryOperationResult.UnknownError;
            }
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