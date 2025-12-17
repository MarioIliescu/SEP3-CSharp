using ApiContracts.Dtos.RecruitDriver;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dispatcher;
using Services.RecruitDriver;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("recruit-driver")]
public class RecruitDriverController(IRecruitDriverService recruitDriverService, IDispatcherService dispatcherService)
    : ControllerBase
{
    // POST recruit
    [HttpPost]
    public async Task<IActionResult> RecruitDriver([FromBody] RecruitDriverDto dto)
    {
        var dispatcher = await dispatcherService.GetSingleAsync(dto.DispatcherId);

        var driver = new Driver.Builder().SetId(dto.DriverId).Build();
        
        var created = await recruitDriverService.RecruitDriverAsync(driver, dispatcher);

        return Ok(created);
    }
    
    // DELETE fire driver
    [HttpDelete ("delete")]
    [Authorize]
    public async Task<IActionResult> FireDriver([FromBody] RecruitDriverDto dto)
    {
        var userIdClaim = User.FindFirst("Id")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        
        var dispatcher = await dispatcherService.GetSingleAsync(dto.DispatcherId);

        if (dispatcher.Id != userId)
            return Forbid();
        
        var driver = new Driver.Builder().SetId(dto.DriverId).Build();
        
        await recruitDriverService.FireDriverAsync(driver, dispatcher);

        return NoContent();
    }
    
    // GET all drivers for dispatcher
    [HttpGet("dispatcher/{id:int}")]
    public async Task<IActionResult> GetDriversForDispatcher(int id)
    {

        var dispatcher = await dispatcherService.GetSingleAsync(id);
        
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