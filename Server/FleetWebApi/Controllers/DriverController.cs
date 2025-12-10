using ApiContracts.Dtos.Driver;
using Entities;
using FleetWebApi.SecurityUtils;
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
            var driver = new Driver.Builder()
                .SetFirstName(dto.FirstName)
                .SetLastName(dto.LastName)
                .SetEmail(dto.Email)
                .SetPhoneNumber(dto.PhoneNumber)
                .SetPassword(PasswordHasher.Hash(dto.Password))
                .SetMcNumber(dto.McNumber)
                .SetTrailerType(dto.TrailerType)
                .SetStatus(dto.StatusType)
                .SetCompanyRole(dto.CompanyRole)
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

    [HttpPut]
    public async Task<IActionResult> UpdateDriverAsync(
        [FromBody] DriverDto dto)
    {
        try
        {
            var driver = new Driver.Builder()
                .SetId(dto.Id)
                .SetFirstName(dto.FirstName)
                .SetLastName(dto.LastName)
                .SetEmail(dto.Email)
                .SetPhoneNumber(dto.PhoneNumber)
                .SetPhotoUrl(dto.PhotoUrl)
                .SetMcNumber(dto.McNumber)
                .SetTrailerType(dto.TrailerType)
                .SetStatus(dto.Status)
                .SetCompanyRole(dto.CompanyRole)
                .SetLocationState(dto.LocationState)
                .SetLocationZip(dto.LocationZip)
                .Build();
            await _driverService.UpdateAsync(driver);
            return NoContent();
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
            driver.PhotoUrl,
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
                d.PhotoUrl,
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