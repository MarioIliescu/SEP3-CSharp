using ApiContracts.Dtos.Dispatcher;
using ApiContracts.Enums;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dispatcher;
using Services.SecurityUtils;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("dispatcher")]
public class DispatcherController : ControllerBase
{
    private readonly IDispatcherService _dispatcherService;
    
    public DispatcherController(IDispatcherService dispatcherService)
    {
        _dispatcherService = dispatcherService;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateDispatcher([FromBody] CreateDispatcherDto dto)
    {
        try
        {
            var dispatcher = new Dispatcher.Builder()
                .SetId(dto.Id)
                .SetFirstName(dto.FirstName)
                .SetLastName(dto.LastName)
                .SetEmail(dto.Email)
                .SetPhoneNumber(dto.PhoneNumber)
                .SetPassword(PasswordHasher.Hash(dto.Password))
                .SetCurrentRate(dto.CurrentRate)
                .Build();

           var created = await _dispatcherService.CreateAsync(dispatcher);

            return Created();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateDispatcherAsync(
        [FromBody] DispatcherDto dto)
    {
        var userIdClaim = User.FindFirst("Id")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        
        var entity = await _dispatcherService.GetSingleAsync(dto.Id);

        if (entity.Id != userId)
            return Forbid();
        
        try
        {
            var dispatcher = new Dispatcher.Builder()
                .SetId(userId)
                .SetFirstName(dto.FirstName)
                .SetLastName(dto.LastName)
                .SetEmail(dto.Email)
                .SetPhoneNumber(dto.PhoneNumber)
                .SetCurrentRate(dto.CurrentRate)
                .SetPhotoUrl(dto.PhotoUrl)
                .Build();
            await _dispatcherService.UpdateAsync(dispatcher);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DispatcherDto>> GetDispatcherById(int id)
    {
        Dispatcher? dispatcher = await _dispatcherService.GetSingleAsync(id);

        if (dispatcher == null)
        {
            return NotFound($"Dispatcher with ID {id} not found.");
        }

        var dto = new DispatcherDto(
            dispatcher.Id,
            dispatcher.FirstName,
            dispatcher.LastName,
            dispatcher.Email,
            dispatcher.PhoneNumber,
            dispatcher.Current_Rate,
            dispatcher.PhotoUrl
        );

        return Ok(dto);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDispatcher(int id)
    {
        var userIdClaim = User.FindFirst("Id")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        
        var entity = await _dispatcherService.GetSingleAsync(id);

        if (entity.Id != userId)
            return Forbid();
        
        Dispatcher? dispatcher = await _dispatcherService.GetSingleAsync(id);
        if (dispatcher == null)
        {
            return NotFound($"Dispatcher with ID {id} not found.");
        }

        await _dispatcherService.DeleteAsync(id);
        return NoContent();
    }
    
    
    [HttpGet]
    public async Task<ActionResult> GetAllDispatchers()
    {

        var dispatcherList = _dispatcherService.GetManyAsync();
        var dispatchersDto = dispatcherList
            .Select(dispatcher => new DispatcherDto(dispatcher.Id,
                dispatcher.FirstName,
                dispatcher.LastName,
                dispatcher.Email,
                dispatcher.PhoneNumber,
                dispatcher.Current_Rate,
                dispatcher.PhotoUrl))
            .ToList();

        return Ok(dispatchersDto);
    }
    
    

}