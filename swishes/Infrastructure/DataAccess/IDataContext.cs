﻿namespace swishes.Infrastructure.DataAccess
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public interface IDataContext
    {
        IDbSet<T> Set<T>() where T : class;
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        bool SetEntryState<T>(T entity, EntityState state) where T : class;
        T GetAttached<T>(T entity) where T : class;
        int SaveChanges();
        void Detach<T>(T entity) where T : class;
        void Dispose();
    }
}