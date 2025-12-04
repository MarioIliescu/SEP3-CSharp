using ApiContracts.Dtos.RecruitDriver;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services.RecruitDriver;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("recruit-driver")]
public class RecruitDriverController(IRecruitDriverService recruitDriverService)
    : ControllerBase
{
    // POST recruit
    [HttpPost]
    public async Task<IActionResult> RecruitDriver([FromBody] RecruitDriverDto dto)
    {
        var driver = new Driver.Builder().SetId(dto.DriverId).Build();
        var dispatcher = new Dispatcher.Builder().SetId(dto.DispatcherId).Build();

        var created = await recruitDriverService.RecruitDriverAsync(driver, dispatcher);

        return Ok(created);
    }
    
    // DELETE fire driver
    [HttpDelete ("delete")]
    public async Task<IActionResult> FireDriver([FromBody] RecruitDriverDto dto)
    {
        var driver = new Driver.Builder().SetId(dto.DriverId).Build();
        var dispatcher = new Dispatcher.Builder().SetId(dto.DispatcherId).Build();
        
        await recruitDriverService.FireDriverAsync(driver, dispatcher);

        return NoContent();
    }
    
    // GET all drivers for dispatcher
    [HttpGet("dispatcher/{id:int}")]
    public async Task<IActionResult> GetDriversForDispatcher(int id)
    {
        var drivers = recruitDriverService.GetDispatcherDriversListAsync(id);
        return Ok(drivers);
    }
    
    // GET all drivers without dispatcher
    [HttpGet("unassigned")]
    public IActionResult GetUnassignedDrivers()
    {
        var drivers = recruitDriverService.GetDriverListWoDispatcherAsync();
        return Ok(drivers);
    }
}