using System.Linq.Expressions;

namespace TheBlog.Data.Database
{
    public interface IRepository<T> where T : class
    {
        T Create(T entity);

        void Delete(T entity);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);

        IQueryable<T> GetAll();

        T Update(T entity);
    }
}