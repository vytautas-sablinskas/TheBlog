using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TheBlog.Data.Database
{
    [ExcludeFromCodeCoverage]
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly TheBlogDbContext _context;
        private readonly DbSet<T> _table;

        public Repository(TheBlogDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public IQueryable<T> GetAll() => _context.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition)
        {
            return _context.Set<T>().Where(condition);
        }

        public T Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _table.Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public T Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _table.Update(entity);
            _context.SaveChanges();

            return entity;
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _table.Remove(entity);
            _context.SaveChanges();
        }
    }
}