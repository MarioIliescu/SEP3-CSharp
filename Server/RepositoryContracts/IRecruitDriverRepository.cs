using Entities;

namespace Repositories;

public interface IRecruitDriverRepository
{
    Task<Driver> RecruitDriverAsync(Driver driver, Dispatcher dispatcher);
    Task FireDriverAsync(Driver driver, Dispatcher dispatcher);
    IQueryable<Driver> GetDispatcherDriversListAsync(int id);
    IQueryable<Driver> GetDriverListWoDispatcherAsync();
}