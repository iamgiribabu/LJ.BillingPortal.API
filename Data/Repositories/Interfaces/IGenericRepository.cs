namespace LJ.BillingPortal.API.Data.Repositories.Interfaces;

/// <summary>
/// Generic repository interface for common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Get entity by primary key
    /// </summary>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Add new entity
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Add multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Update entity
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete entity
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Delete entity by primary key
    /// </summary>
    Task DeleteByIdAsync(object id);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(object id);

    /// <summary>
    /// Save changes to database
    /// </summary>
    Task<int> SaveChangesAsync();
}
