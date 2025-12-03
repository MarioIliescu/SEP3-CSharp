using Entities;

namespace Repositories;

public interface IRecruitDriverRepository
{
    Driver RecruitDriverAsync(Driver driver, Dispatcher dispatcher);
    void FireDriverAsync(Driver driver, Dispatcher dispatcher);
    IQueryable<Driver> GetDispatcherDriversListAsync(int id);
    IQueryable<Driver> GetDriverListWoDispatcherAsync();
}