using ApiContracts.Dtos.Job;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Job;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("job")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    
    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
    {
        try
        {
            var job = new Job.Builder()
                .SetDispatcherId(dto.DispatcherId)
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
                job.jobId,
                job.dispatcherId,
                job.driverId,
                job.Title,
                job.Description,
                job.loaded_miles,
                job.weight_of_cargo,
                job.type_of_trailer_needed,
                job.total_price,
                job.cargo_info,
                job.pickup_time,
                job.delivery_time,
                job.pickup_location_state,
                job.pickup_location_zip,
                job.drop_location_state,
                job.drop_location_zip,
                job.current_status
            );
            return Ok(dto);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteJob(int id)
    {
        Job? job = await _jobService.GetSingleAsync(id);
        if (job == null)
        {
            return NotFound($"Job with ID {id} not found.");
        }

        await _jobService.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAllJobs()
    {

        var jobsList = _jobService.GetManyAsync();
        var jobsDto = jobsList
            .Select(j => new JobDto(
                j.jobId,
                j.dispatcherId,
                j.driverId,
                j.Title,
                j.Description,
                j.loaded_miles,
                j.weight_of_cargo,
                j.type_of_trailer_needed,
                j.total_price,
                j.cargo_info,
                j.pickup_time,
                j.delivery_time,
                j.pickup_location_state,
                j.pickup_location_zip,
                j.drop_location_state,
                j.drop_location_zip,
                j.current_status))
            .ToList();

        return Ok(jobsDto);
    }

}