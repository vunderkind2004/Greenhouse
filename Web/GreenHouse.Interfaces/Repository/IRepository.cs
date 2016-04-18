using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.Repository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        void Create<T>(T entity);
        void Update<T>(T entity);
        void Delete<T>(T entity);
    }
}
