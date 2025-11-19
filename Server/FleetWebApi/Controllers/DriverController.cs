using ApiContracts.Dtos.Driver;
using ApiContracts.Enums;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Driver;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("driver")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;
    
    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateDriver([FromBody] CreateDriverDto dto)
    {
        try
        {
            if (!Enum.TryParse<TrailerType>(dto.TrailerType, true, out var trailerType))
                return BadRequest("Invalid trailer type");

            var driver = new Driver.Builder()
                .SetFirstName(dto.FirstName)
                .SetLastName(dto.LastName)
                .SetEmail(dto.Email)
                .SetPhoneNumber(dto.PhoneNumber)
                .SetPassword(dto.Password)
                .SetMcNumber(dto.McNumber)
                .SetTrailerType(trailerType)
                .SetLocationState(dto.LocationState)
                .SetLocationZip(dto.LocationZipCode)
                .Build();

           var created = await _driverService.CreateAsync(driver);

            return Created();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DriverDto>> GetDriverById(int id)
    {
        Driver? driver = await _driverService.GetSingleAsync(id);

        if (driver == null)
        {
            return NotFound($"Driver with ID {id} not found.");
        }

        var dto = new DriverDto(
            driver.Id,
            driver.FirstName,
            driver.LastName,
            driver.Email,
            driver.PhoneNumber,
            driver.Password,
            driver.Role,
            driver.McNumber,
            driver.Status,
            driver.Trailer_type,
            driver.Location_State,
            driver.Location_Zip_Code,
            driver.CompanyRole
        );

        return Ok(dto);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDriver(int id)
    {
        Driver? driver = await _driverService.GetSingleAsync(id);
        if (driver == null)
        {
            return NotFound($"Driver with ID {id} not found.");
        }

        await _driverService.DeleteAsync(id);
        return NoContent();
    }
    
    
    [HttpGet]
    public async Task<ActionResult> GetAllDrivers()
    {

        var driversList = _driverService.GetManyAsync();
        var driversDto = driversList
            .Select(d => new DriverDto(d.Id,
                d.FirstName,
                d.LastName,
                d.Email,
                d.PhoneNumber,
                d.Password,
                d.Role,
                d.McNumber,
                d.Status,
                d.Trailer_type,
                d.Location_State,
                d.Location_Zip_Code,
                d.CompanyRole))
            .ToList();

        return Ok(driversDto);
    }
    
    

}