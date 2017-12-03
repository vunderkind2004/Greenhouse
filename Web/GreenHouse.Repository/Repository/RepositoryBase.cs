using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.Repository;
using ServiceStack.OrmLite;

namespace GreenHouse.Repository.Repository
{
    public class RepositoryBase<T> : IRepository<T>
    {
        private readonly string connectionString;

        public RepositoryBase(string connectionString)
        {
            this.connectionString = connectionString;
        }
    
        #region IRepository<T> Members

        public IEnumerable<T> GetAll()
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                var result = db.Select<T>();
                return result;
            }
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                var result = await db.SelectAsync<T>();
                return result;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicte)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                var result = await db.SelectAsync<T>(predicte);
                return result;
            }
        }

        public void Create<T>(T entity)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                db.Insert(entity);                
            }
        }

        public void Update<T>(T entity)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                db.Update(entity);
            }
        }

        public void Delete<T>(T entity)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                db.Delete(entity);
            }
        }

        #endregion

        private OrmLiteConnectionFactory GetFactory()
        {
            return new OrmLiteConnectionFactory( connectionString,SqlServerDialect.Provider);
        }

        public IQueryable<T> Select(Expression<Func<T, bool>> predicate)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                return db.Select<T>(predicate).AsQueryable<T>();               
            }
        }

        public T Single(Expression<Func<T, bool>> predicate)
        {
            var factory = GetFactory();
            using (var db = factory.Open())
            {
                return db.Single<T>(predicate);
            }
        }


    }
}
