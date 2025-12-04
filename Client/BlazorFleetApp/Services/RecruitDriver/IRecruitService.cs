using ApiContracts.Dtos.Driver;
using ApiContracts.Dtos.RecruitDriver;

namespace BlazorFleetApp.Services.RecruitDriver;

public interface IRecruitService
{
    Task RecruitDriver(RecruitDriverDto dto);
    Task FireDriver(RecruitDriverDto dto);
    Task<List<DriverDto>> GetDriversForDispatcher(int dispatcherId);
    Task<List<DriverDto>> GetDrivers();
}