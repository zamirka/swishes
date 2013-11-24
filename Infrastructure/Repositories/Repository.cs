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

        public virtual RepositoryOperationStatus Add(T entity)
        {
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
                return RepositoryOperationStatus.UnknownError;
            }
            return countAfter > countBefore ? RepositoryOperationStatus.Success : RepositoryOperationStatus.EntityAlreadyExists;
        }

        public virtual RepositoryOperationStatus Delete(T entity)
        {
            try
            {
                if (_dbset.Remove(entity) != null)
                {
                    var entry = _context.Entry(entity);
                    entry.State = EntityState.Deleted;
                    return RepositoryOperationStatus.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("Error occured in Repository<{0}>.Delete(entity)", typeof(T).ToString()), ex);
                return RepositoryOperationStatus.UnknownError;
            }
            return RepositoryOperationStatus.EntityNotFound;
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
            try
            {
                var entry = _context.Entry(entity);
                if (_dbset.Attach(entity) != null)
                {
                    entry.State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("Error occured in Repository<{0}>.Delete(entity)", typeof(T).ToString()), ex);
                return RepositoryOperationStatus.UnknownError;
            }
            return RepositoryOperationStatus.EntityNotFound;
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