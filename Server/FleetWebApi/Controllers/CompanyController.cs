using Entities;
using ApiContracts.Company;
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
            .Select(c => new CreateCompanyDto(c.Id,c.McNumber, c.CompanyName))
            .ToList();

        return Ok(companiesDto);
    }

// GET /company/{mcNumber}
    [HttpGet("{mcNumber}")]
    public async Task<ActionResult<CreateCompanyDto>> GetSingleCompanyAsync(string mcNumber)
    {
        var company = await _companyService.GetSingleAsync(mcNumber);

        return Ok(new CreateCompanyDto(company.Id,company.McNumber, company.CompanyName));
    }


    // POST /company
    [HttpPost]
    public async Task<ActionResult> CreateCompanyAsync([FromBody] CreateCompanyDto dto)
    {
        var company = new Company.Builder()
            .SetMcNumber(dto.McNumber)
            .SetCompanyName(dto.CompanyName)
            .Build();

        Company created = await _companyService.CreateAsync(company);
        return CreatedAtAction(nameof(GetSingleCompanyAsync), new { mcNumber = created.McNumber }, new 
            CreateCompanyDto(created.Id,created.McNumber, created.CompanyName));
    }

    // PUT /company
    [HttpPut]
    public async Task<ActionResult> UpdateCompanyAsync([FromBody]  CreateCompanyDto dto)
    {
        var existing = await _companyService.GetSingleAsync(dto.Id);
        var company = new Company.Builder()
            .SetId(existing.Id)
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