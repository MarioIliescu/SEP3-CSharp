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
    public async Task<ActionResult> GetCompanies()
    {
        var companiesList = _companyService.GetManyAsync();
        var companiesDto = companiesList
            .Select(c => new CreateCompanyDto(c.Id,c.McNumber, c.CompanyName))
            .ToList();

        return Ok(companiesDto);
    }

// GET /company/{mcNumber}
    [HttpGet("{mcNumber}")]
    public async Task<ActionResult<CreateCompanyDto>> GetSingleCompany(string mcNumber)
    {
        var company = await _companyService.GetSingleAsync(mcNumber);
        if (company == null)
            return NotFound("Company not found");

        return Ok(new CreateCompanyDto(company.Id,company.McNumber, company.CompanyName));
    }


    // POST /company
    [HttpPost]
    public async Task<ActionResult> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        var company = new Company.Builder()
            .SetMcNumber(dto.McNumber)
            .SetCompanyName(dto.CompanyName)
            .Build();

        await _companyService.CreateAsync(company);
        return CreatedAtAction(nameof(GetSingleCompany), new { mcNumber = dto.McNumber }, dto);
    }

    // PUT /company
    [HttpPut]
    public async Task<ActionResult> UpdateCompany([FromBody]  CreateCompanyDto dto)
    {
        var existing = await _companyService.GetSingleAsync(dto.Id);
        if (existing == null)
            return NotFound("Company not found");

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
    public async Task<ActionResult> DeleteCompany(string mcNumber)
    {
        var existing = await _companyService.GetSingleAsync(mcNumber);
        if (existing == null)
            return NotFound("Company not found");

        await _companyService.DeleteAsync(mcNumber);
        return NoContent();
    }

}
