using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.Repository;
using ServiceStack.OrmLite;

namespace GreenHouse.Repository.Repository
{
    public class RepositoryBase<T> : IRepository<T>
    {
        private string connectionString;

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
    }
}
