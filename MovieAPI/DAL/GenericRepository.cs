using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MovieAPI.DAL;

public class GenericRepository<TEntity> where TEntity : class
{
    internal MovieApiContext _context;
    internal DbSet<TEntity> _dbSet;

    public GenericRepository(MovieApiContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<ICollection<TEntity>> Get(
        Expression<Func<TEntity,bool>>? filter = null,
        Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet;                                                                  // Creates the base for the query. Uses the model that instantiated the GenericRepository

        if (filter != null) query = query.Where(filter);                                                     // Uses filter parameter to add Where statement to query
                                                                                                             // Can only use one filter

        string[] properties = includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries);          // Parses includedProperties parameter
        foreach (var property in properties)                                                                 // Adds the newly parsed properties to the query using the Include method
        {                                                                                                    // Acts as a join statement
            query = query.Include(property);
        }


        List<TEntity> result;
        if (orderBy != null)                                                                                 // orderBy parameter passed in as IQueryable:
        {                                                                                                    // q => q.OrderBy(s => s.PROPERTY) or q.OrderByDescending(...)
            result = await orderBy(query).ToListAsync();                                                     // q = query; The query that is being built inside this function
        }
        else
        {
            result = await query.ToListAsync();
        }

        return result;
    }

    public virtual async Task<TEntity?> GetById(Expression<Func<TEntity, bool>>? filter = null, string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null) query = query.Where(filter);

        string[] properties = includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries);          // Parses includedProperties parameter
        foreach (var property in properties)                                                                 // Adds the newly parsed properties to the query using the Include method
        {                                                                                                    // Acts as a join statement
            query = query.Include(property);
        }


        TEntity? result = await query.FirstOrDefaultAsync();

        return result;
    }

    public virtual void Insert(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Added;
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual bool Exists(Expression<Func<TEntity,bool>> predicate)
    {
        bool entityExists = _dbSet.Any(predicate);
        return entityExists;
    }
}
