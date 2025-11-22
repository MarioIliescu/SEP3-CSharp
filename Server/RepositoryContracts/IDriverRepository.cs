using Entities;

namespace Repositories;

/// <summary>
/// Defines CRUD operations for managing <see cref="Driver"/> entities.
/// </summary>
public interface IDriverRepository
{
    
    /// <summary>
    /// Creates a new driver.
    /// </summary>
    /// <param name="payload">The driver to create.</param>
    /// <returns>The created <see cref="Driver"/>.</returns>
    Task<Driver> CreateAsync(Driver payload);
    
    /// <summary>
    /// Updates an existing driver.
    /// </summary>
    /// <param name="payload">The driver to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Driver payload);
    
    
    /// <summary>
    /// Retrieves a single driver by its id.
    /// </summary>
    /// <param name="id">The id of the driver to retrieve.</param>
    /// <returns>The matching <see cref="Driver"/> or <c>null</c> if not found.</returns>
    Task<Driver> GetSingleAsync(int id);
    
    /// <summary>
    /// Deletes a driver by its id.
    /// </summary>
    /// <param name="id">The id of the driver to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Retrieves all drivers.
    /// </summary>
    /// <returns>An <see cref="IQueryable{Driver}"/> containing all companies.</returns>
    IQueryable<Driver>GetManyAsync();
}