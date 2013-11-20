using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace swishes.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        RepositoryOperationStatus Add(T entity);
        RepositoryOperationStatus Delete(T entity);
        RepositoryOperationStatus DeleteAll(IEnumerable<T> entity);
        RepositoryOperationStatus Update(T entity);
        bool Any();
    }
}