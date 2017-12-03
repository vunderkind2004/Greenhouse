using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.Repository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicte);
        void Create<T>(T entity);
        void Update<T>(T entity);
        void Delete<T>(T entity);
        IQueryable<T> Select(Expression<Func<T, bool>> predicate);
        T Single(Expression<Func<T, bool>> predicate);
    }
}
