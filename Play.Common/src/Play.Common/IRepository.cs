

using System.Linq.Expressions;
using Play.Common.Entities;

namespace Play.Common.Repositories
{
    public interface IRepository<T> where T :IEntity
    {
       public Task<IReadOnlyCollection<T>> GetAllAsync();
        public Task<IReadOnlyCollection<T>> GetAllAsync(Expression <Func<T,bool>> filter);
       public Task<T> GetAsync(Guid id);
        public Task<T> GetAsync(Expression <Func<T,bool>> filter);
       public Task CreatedAsync(T entity);
       public Task UpdateAsync(T entity);
       public Task RemoveAsync(Guid id);
    }
}