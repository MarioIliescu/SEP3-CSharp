# WebAPI

- [WebAPI](#webapi)
  - [NuGet used](#nuget-used)
  - [File System Overview](#file-system-overview)
  - [Program code (Entry point)](#program-code-entry-point)
  - [Controler, Restful approach](#controler-restful-approach)
  - [Services](#services)
    - [Interface](#interface)
    - [Implementation](#implementation)
  - [Launch settings](#launch-settings)

For a better overview of the project go back to [README](https://github.com/MarioIliescu/SEP3-CSharp/blob/master/README.md)  

## NuGet used

<img width="548" height="138" alt="Microsoft.AspNetCore.OpenApi" src="https://github.com/user-attachments/assets/30a6ece4-3856-4a9f-b708-0fbac7973438" />

## File System Overview

<img width="394" height="308" alt="FileSystem" src="https://github.com/user-attachments/assets/c775edd0-b7ab-49fa-8068-e7ebac8b8b4a" />

## Program code (Entry point)

```C#
using GrpcAPI.Services;
using PersistanceContracts;
using PersistanceHandlersGrpc.CompanyPersistance;
using Services.Company;

var builder = WebApplication.CreateBuilder(args);
//Add controllers to the container 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//add more services
builder.Services.AddScoped<CompanyServiceProto>();
builder.Services.AddScoped<IFleetPersistanceHandler, CompanyHandlerGrpc>();
builder.Services.AddScoped<ICompanyService, CompanyService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
```

## Controler, Restful approach

```C#
using Entities;
using ApiContracts;
using Microsoft.AspNetCore.Mvc;
using Services.Company;

namespace FleetWebApi.Controllers;

[ApiController]//must have
[Route("company")]//url layout
//must implement Controller base
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    //Service used in constructor
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
            .Select(c => new CompanyDto(c.McNumber, c.CompanyName))
            .ToList();

        return Ok(companiesDto);
    }

// GET /company/{mcNumber}
    [HttpGet("{mcNumber}")]
    public async Task<ActionResult<CompanyDto>> GetSingleCompany(string mcNumber)
    {
        var company = await _companyService.GetSingleAsync(mcNumber);
        if (company == null)
            return NotFound("Company not found");

        return Ok(new CompanyDto(company.McNumber, company.CompanyName));
    }


    // POST /company
    [HttpPost]
    public async Task<ActionResult> CreateCompany([FromBody] CompanyDto dto)
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
    public async Task<ActionResult> UpdateCompany([FromBody] CompanyDto dto)
    {
        var existing = await _companyService.GetSingleAsync(dto.McNumber);
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
```

## Services

Used for business logic.

### Interface

```C#
using Repositories;
namespace Services.Company;
using Entities;
    /// <summary>
    /// Defines CRUD operations for managing <see cref="Company"/> entities.
    /// </summary>
    //Extends Repo
    public interface ICompanyService : ICompanyRepository
    {
        //Add new if you need extra methods besides CRUD, for example server side calculations and so on
    }
```

### Implementation

```C#
namespace Services.Company;
using ApiContracts;
using ApiContracts.Enums;
using PersistanceContracts;
using Entities;
public class CompanyService :ICompanyService
{
    //Handler
    private readonly IFleetPersistanceHandler _handler;

    public CompanyService(IFleetPersistanceHandler handler)
    {
        this._handler = handler;
    }
    public async Task<Company> CreateAsync(Company payload)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Create, payload);
        return (Company) await  _handler.HandleAsync(request);
    }

    public async Task UpdateAsync(Company payload)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Update, payload);
        await  _handler.HandleAsync(request);
    }

    public async Task<Company> GetSingleAsync(string mcNumber)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Get, 
            new Company.Builder()
            .SetMcNumber(mcNumber)
            .Build());
        return (Company) await _handler.HandleAsync(request);
    }

    public async Task<Company> GetSingleAsync(int id)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Get, 
            new Company.Builder()
                .SetId(id)
                .Build());
        return (Company) await _handler.HandleAsync(request);
    }

    public async Task DeleteAsync(string mcNumber)
    {
        Request request = MakeCompanyRequest(ActionType.Delete, 
            new Company.Builder()
                .SetMcNumber(mcNumber)
                .Build());
         await _handler.HandleAsync(request);
    }

    public async Task DeleteAsync(int id)
    {
        Request request = MakeCompanyRequest(ActionType.Delete, 
            new Company.Builder()
                .SetId(id)
                .Build());
        await _handler.HandleAsync(request);
    }

    public IQueryable<Company> GetManyAsync()
    {
        Request request = MakeCompanyRequest(ActionType.List, new Company.Builder()
            .SetId(0)
            .Build());

        var result = _handler.HandleAsync(request).Result as IQueryable<Company> ;
        if (result != null)
        {
            return result;
        }
        throw new InvalidOperationException("No companies found");
    }
    //Helper method since HandlerType.Company is always the same
    private Request MakeCompanyRequest(ActionType action, Company company)
    {
        return new Request(action, HandlerType.Company, company);
    }
}
```

## Launch settings

Make sure the ports are different for the Api and Blazor on startup.  
Make sure to send the `BlazorClient` to the Api port, either http or https.

```Json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:7032",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7191;http://localhost:7032",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```
