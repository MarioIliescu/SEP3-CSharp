using Entities;

namespace Repositories;


/// <summary>
/// Defines CRUD operations for managing <see cref="Dispatcher"/> entities.
/// </summary>
public interface IDispatcherRepository
{
    /// <summary>
    /// Creates a new dispatcher.
    /// </summary>
    /// <param name="payload">The dispatcher to create.</param>
    /// <returns>The created <see cref="Dispatcher"/>.</returns>
    Task<Dispatcher> CreateAsync(Dispatcher payload);
    
    
    /// <summary>
    /// Updates an existing dispatcher.
    /// </summary>
    /// <param name="payload">The dispatcher to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Dispatcher payload);
    
    
    /// <summary>
    /// Retrieves a single dispatcher by its id.
    /// </summary>
    /// <param name="id">The id of the dispatcher to retrieve.</param>
    /// <returns>The matching <see cref="Dispatcher"/> or <c>null</c> if not found.</returns>
    Task<Dispatcher> GetSingleAsync(int id);
    
    /// <summary>
    /// Deletes a dispatcher by its id.
    /// </summary>
    /// <param name="id">The id of the dispatcher to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Retrieves all dispatchers.
    /// </summary>
    /// <returns>An <see cref="IQueryable{Dispatcher}"/> containing all companies.</returns>
    IQueryable<Dispatcher>GetManyAsync();


}