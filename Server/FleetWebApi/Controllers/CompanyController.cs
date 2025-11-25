using Entities;
using ApiContracts.Company;
using ApiContracts.Dtos.Driver;
using Microsoft.AspNetCore.Mvc;
using Services.Company;

namespace FleetWebApi.Controllers;

[ApiController]
[Route("company")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    // GET /company
    [HttpGet]
    public async Task<ActionResult> GetCompaniesAsync()
    {
        var companiesList = _companyService.GetManyAsync();
        var companiesDto = companiesList
            .Select(c => new CompanyDto(c.McNumber, c.CompanyName))
            .ToList();

        return Ok(companiesDto);
    }

// GET /company/{mcNumber}
    [HttpGet("{mcNumber}")]
    public async Task<ActionResult<CompanyDto>> GetSingleCompanyAsync(string mcNumber)
    {
        var company = await _companyService.GetSingleAsync(mcNumber);

        return Ok(new CompanyDto(company.McNumber, company.CompanyName));
    }


    // POST /company
    [HttpPost]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] CreateCompanyDto dto)
    {
        var company = new Company.Builder()
            .SetMcNumber(dto.McNumber)
            .SetCompanyName(dto.CompanyName)
            .Build();
        try
        {
            var created = await _companyService.CreateAsync(company);
            if (string.IsNullOrWhiteSpace(created?.McNumber))
                return BadRequest("Created entity is missing MC number");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Created();
    }

    // PUT /company
    [HttpPut]
    public async Task<ActionResult> UpdateCompanyAsync([FromBody]  CreateCompanyDto dto)
    {
        var company = new Company.Builder()
            .SetMcNumber(dto.McNumber)
            .SetCompanyName(dto.CompanyName)
            .Build();

        await _companyService.UpdateAsync(company);
        return NoContent();
    }

    // DELETE /company/{mcNumber}
    [HttpDelete("{mcNumber}")]
    public async Task<ActionResult> DeleteCompanyAsync(string mcNumber)
    {
        var existing = await _companyService.GetSingleAsync(mcNumber);
        if (existing == null)
            return NotFound("Company not found");

        await _companyService.DeleteAsync(mcNumber);
        return NoContent();
    }
    
    
    

}