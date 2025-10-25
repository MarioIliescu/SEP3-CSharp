using Repositories;
namespace Services.Company;
using Entities;
    /// <summary>
    /// Defines CRUD operations for managing <see cref="Company"/> entities.
    /// </summary>
    public interface ICompanyService : ICompanyRepository
    {
        //Add new if you need extra methods besides CRUD, for example server side calculations and so on
    }
