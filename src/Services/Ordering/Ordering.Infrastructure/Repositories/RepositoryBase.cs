using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class RepositoryBase<T>: IAsyncRepository<T> where T: EntityBase
    {
        protected readonly OrderContext DbContext;

        public RepositoryBase(OrderContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            // the OrderContext represent a database not a table, one of the tables that we defined is Orders but there can be others in the general case.
            // therefore in order to be generic and avoid using _dbContext.Orders (which we named) the .Set<Order> will use the DbSet<Order>. 
            return await DbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = DbContext.Set<T>();
            if (!string.IsNullOrWhiteSpace(includeString))
                query = query.Include(includeString);

            if (predicate != null) 
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = DbContext.Set<T>();
            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await DbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await WithSave(()=>DbContext.Set<T>().Add(entity));
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            await WithSave(() => DbContext.Entry(entity).State = EntityState.Modified);
        }

        public async Task DeleteAsync(T entity)
        {
            await WithSave(()=> DbContext.Set<T>().Remove(entity));
        }

        private async Task WithSave(Action action)
        {
            action();
            await DbContext.SaveChangesAsync();
        }
    }
}
