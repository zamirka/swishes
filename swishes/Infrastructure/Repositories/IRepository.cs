using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace swishes.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        RepositoryOperationResult Add(T entity);
        RepositoryOperationResult Delete(T entity);
        RepositoryOperationResult DeleteAll(IEnumerable<T> entity);
        RepositoryOperationResult Update(T entity);
        bool Any();
    }
}