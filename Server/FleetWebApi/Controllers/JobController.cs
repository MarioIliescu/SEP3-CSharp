using ApiContracts.Dtos.Job;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dispatcher;
using Services.Driver;
using Services.Job;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("job")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IDispatcherService _dispatcherService;
    
    public JobController(IJobService jobService, IDispatcherService dispatcherService)
    {
        _jobService = jobService;
        _dispatcherService = dispatcherService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
    {
        try
        { 
            var userIdClaim = User.FindFirst("Id")?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();
        
            var entity = await _dispatcherService.GetSingleAsync(dto.DispatcherId);

            if (entity.Id != userId)
                return Forbid();
            
            var job = new Job.Builder()
                .SetDispatcherId(userId)
                .SetTitle(dto.Title)
                .SetDescription(dto.Description)
                .SetLoadedMiles(dto.loadedMiles)
                .SetWeight(dto.weightOfCargo)
                .SetTrailerType(dto.TypeOfTrailerNeeded)
                .SetTotalPrice(dto.TotalPrice)
                .SetCargoInfo(dto.CargoInfo)
                .SetPickupTime(dto.PickupTime)
                .SetDeliveryTime(dto.DeliveryTime)
                .SetPickupState(dto.PickupLocationState)
                .SetPickupZip(dto.PickupLocationZip)
                .SetDropState(dto.DropLocationState)
                .SetDropZip(dto.DropLocationZip)
                .Build();
            
            var created = await _jobService.CreateAsync(job);

            return Created();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);

        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobDto>> GetJobById([FromRoute] int id)
    {
       Job? job = await _jobService.GetSingleAsync(id);
       
         if (job == null)
         {
              return NotFound($"Job with Id {id} not found.");
         }
            var dto = new JobDto(
                job.JobId,
                job.DispatcherId,
                job.DriverId,
                job.Title,
                job.Description,
                job.Loaded_Miles,
                job.Weight_Of_Cargo,
                job.Type_Of_Trailer_Needed,
                job.Total_Price,
                job.Cargo_Info,
                job.Pickup_Time,
                job.Delivery_Time,
                job.Pickup_Location_State,
                job.Pickup_Location_Zip,
                job.Drop_Location_State,
                job.Drop_Location_Zip,
                job.Current_Status
            );
            return Ok(dto);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult> DeleteJob(int id)
    {
        var userIdClaim = User.FindFirst("Id")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        
        var entity = await _dispatcherService.GetSingleAsync(userId);

        if (entity.Id != userId)
            return Forbid();
        
        Job? job = await _jobService.GetSingleAsync(id);
        if (job == null)
        {
            return NotFound($"Job with ID {id} not found.");
        }

        await _jobService.DeleteAsync(id);
        return NoContent();
    }
    
    // PUT /job
    [HttpPut]
    public async Task<ActionResult> UpdateJobAsync([FromBody] UpdateJobDto dto)
    {
        try
        {
            var updatedJob = new Job.Builder()
                .SetId(dto.Id)
                .SetDispatcherId(dto.DispatcherId)
                .SetDriverId(dto.DriverId)
                .SetTitle(dto.Title)
                .SetDescription(dto.Description)
                .SetLoadedMiles(dto.LoadedMiles)
                .SetWeight(dto.WeightOfCargo)
                .SetTrailerType(dto.TypeOfTrailerNeeded)
                .SetTotalPrice(dto.TotalPrice)
                .SetCargoInfo(dto.CargoInfo)
                .SetPickupTime(dto.PickupTime)
                .SetDeliveryTime(dto.DeliveryTime)
                .SetPickupState(dto.PickupLocationState)
                .SetPickupZip(dto.PickupLocationZip)
                .SetDropState(dto.DropLocationState)
                .SetDropZip(dto.DropLocationZip)
                .SetStatus(dto.CurrentStatus)
                .Build();

            await _jobService.UpdateAsync(updatedJob);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetAllJobs()
    {

        var jobsList = _jobService.GetManyAsync();
        var jobsDto = jobsList
            .Select(j => new JobDto(
                j.JobId,
                j.DispatcherId,
                j.DriverId,
                j.Title,
                j.Description,
                j.Loaded_Miles,
                j.Weight_Of_Cargo,
                j.Type_Of_Trailer_Needed,
                j.Total_Price,
                j.Cargo_Info,
                j.Pickup_Time,
                j.Delivery_Time,
                j.Pickup_Location_State,
                j.Pickup_Location_Zip,
                j.Drop_Location_State,
                j.Drop_Location_Zip,
                j.Current_Status))
            .ToList();

        return Ok(jobsDto);
    }

}