namespace Repositories;
using Entities;

/// <summary>
/// Defines a repository interface for managing <see cref="Company"/> entities.
/// This interface provides asynchronous CRUD (Create, Read, Update, Delete) operations
/// for company data access.
/// </summary>
/// <remarks>
/// The interface supports operations using both MC number and ID as identifiers.
/// The implementation should ensure thread-safe operations and proper error handling.
/// All operations are asynchronous to support scalability.
/// Created on 18-10-2025.
/// </remarks>

public interface ICompanyRepository
{
    /// <summary>
    /// Creates a new company in the repository.
    /// </summary>
    /// <param name="payload">The <see cref="Company"/> object to create.</param>
    /// <returns>The created <see cref="Company"/> object, including any generated properties like Id.</returns>
    Task<Company> CreateAsync(Company payload);

    /// <summary>
    /// Updates an existing company in the repository.
    /// </summary>
    /// <param name="payload">The <see cref="Company"/> object with updated values.</param>
    /// <returns>The updated <see cref="Company"/> object.</returns>
    Task<Company> UpdateAsync(Company payload);

    /// <summary>
    /// Retrieves a single company by its MC number.
    /// </summary>
    /// <param name="mcNumber">The MC number of the company to retrieve.</param>
    /// <returns>The <see cref="Company"/> object if found; otherwise, <c>null</c>.</returns>
    Task<Company?> GetSingleAsync(string mcNumber);

    /// <summary>
    /// Retrieves a single company by its ID.
    /// </summary>
    /// <param name="id">The ID of the company to retrieve.</param>
    /// <returns>The <see cref="Company"/> object if found; otherwise, <c>null</c>.</returns>
    Task<Company?> GetSingleAsync(int id);

    /// <summary>
    /// Deletes a company from the repository by its MC number.
    /// </summary>
    /// <param name="mcNumber">The MC number of the company to delete.</param>
    Task DeleteAsync(string mcNumber);

    /// <summary>
    /// Deletes a company from the repository by its ID.
    /// </summary>
    /// <param name="id">The ID of the company to delete.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Retrieves all companies in the repository.
    /// </summary>
    /// <returns>An <see cref="IQueryable{Company}"/> representing all companies.</returns>
    IQueryable<Company> GetAll();
}
