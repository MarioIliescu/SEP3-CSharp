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
            .Select(c => new CreateCompanyDto(c.McNumber, c.CompanyName))
            .ToList();

        return Ok(companiesDto);
    }

// GET /company/{mcNumber}
    [HttpGet("{mcNumber}")]
    public async Task<ActionResult<CreateCompanyDto>> GetSingleCompanyAsync(string mcNumber)
    {
        var company = await _companyService.GetSingleAsync(mcNumber);

        return Ok(new CreateCompanyDto(company.McNumber, company.CompanyName));
    }


    // POST /company
    [HttpPost]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] CreateCompanyDto dto)
    {
        // 1️⃣  Build the entity
        var company = new Company.Builder()
            .SetMcNumber(dto.McNumber)
            .SetCompanyName(dto.CompanyName)
            .Build();

        // 2️⃣  Persist it
        var created = await _companyService.CreateAsync(company);
        // 4️⃣  Guard
        if (string.IsNullOrWhiteSpace(created?.McNumber))
        {
            return BadRequest("McNumber was not returned by the service.");
        }

        // 5️⃣  Send the 201
        var createdDto = new CreateCompanyDto(created.McNumber, created.CompanyName);
        return CreatedAtAction(nameof(GetSingleCompanyAsync),
            new { mcNumber = created.McNumber },
            createdDto);
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