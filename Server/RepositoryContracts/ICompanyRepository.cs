namespace Repositories;
using Entities;
public interface ICompanyRepository
{
    //CRUD operations
    /**
     * Create a new company
     * @param payload The company to create
     * @return The created company
     */
    Task<Company> CreateAsync(Company payload);
    /**
     * Update an existing company
     * @param payload The company to update
     * @return The updated company
     */
    Task<Company> UpdateAsync(Company payload);

    /**
     * Get a single company by its mcNumber
     * @param mcNumber The mcNumber of the company to get
     * @return The company or null if not found
     */
    Task<Company> GetSingleAsync(String mcNumber);

    /**
     * Get a single company by its id
     * @param id The id of the company to get
     * @return The company or null if not found
     */
    Task<Company> GetSingleAsync(int id);

    /**
     * Delete a company by its mcNumber
     * @param mcNumber The mcNumber of the company to delete
     */
    Task DeleteAsync(String mcNumber);

    /**
     * Delete a company by its id
     * @param id The id of the company to delete
     */
    Task DeleteAsync(int id);

    /**
     * Get all companies
     * @return An iterable of companies
     */
    IQueryable<Company> GetAll();
}