namespace Repositories;
using Entities;
/// <summary>
/// Defines CRUD operations for managing <see cref="Company"/> entities.
/// </summary>
public interface ICompanyRepository
{
    
        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="payload">The company to create.</param>
        /// <returns>The created <see cref="Company"/>.</returns>
        Task<Company> CreateAsync(Company payload);

        /// <summary>
        /// Updates an existing company.
        /// </summary>
        /// <param name="payload">The company to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Company payload);

        /// <summary>
        /// Retrieves a single company by its MC number.
        /// </summary>
        /// <param name="mcNumber">The MC number of the company to retrieve.</param>
        /// <returns>The matching <see cref="Company"/> or <c>null</c> if not found.</returns>
        Task<Company> GetSingleAsync(string mcNumber);
        

        /// <summary>
        /// Deletes a company by its MC number.
        /// </summary>
        /// <param name="mcNumber">The MC number of the company to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string mcNumber);
        

        /// <summary>
        /// Retrieves all companies.
        /// </summary>
        /// <returns>An <see cref="IQueryable{Company}"/> containing all companies.</returns>
        IQueryable<Company>GetManyAsync();
}