using Entities;

namespace Repositories;

public interface IRecruitDriverRepository
{
    Driver RecruitDriver(Driver driver, Dispatcher dispatcher);
    void FireDriver(Driver driver, Dispatcher dispatcher);
    IQueryable<Driver> GetDispatcherDriversList(int id);
    IQueryable<Driver> GetDriverListWoDispatcher();
}