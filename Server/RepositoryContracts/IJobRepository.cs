using Entities;

namespace Repositories;


/// <summary>
/// Defines CRUD operations for managing <see cref="Job"/> entities.
/// </summary>
public interface IJobRepository
{
    /// <summary>
    /// Creates a new job.
    /// </summary>
    /// <param name="payload">The job to create.</param>
    /// <returns>The created <see cref="Job"/>.</returns>
    Task<Job> CreateAsync(Job payload);
    
    /// <summary>
    /// Updates an existing job.
    /// </summary>
    /// <param name="payload">The job to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Job payload);
    
    
    /// <summary>
    /// Retrieves a single job by its id.
    /// </summary>
    /// <param name="id">The id of the job to retrieve.</param>
    /// <returns>The matching <see cref="Job"/> or <c>null</c> if not found.</returns>
    Task<Job> GetSingleAsync(int id);
    
    /// <summary>
    /// Deletes a job by its id.
    /// </summary>
    /// <param name="id">The id of the job to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Retrieves all jobs.
    /// </summary>
    /// <returns>An <see cref="IQueryable{Job}"/> containing all jobs.</returns>
    IQueryable<Job>GetManyAsync();
}