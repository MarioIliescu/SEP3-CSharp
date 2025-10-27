# BlazorClient

- [BlazorClient](#blazorclient)
  - [File System Overview](#file-system-overview)
  - [Program code (Entry point)](#program-code-entry-point)
  - [Client](#client)
  - [Razor Component](#razor-component)
    - [Navbar](#navbar)
    - [App](#app)
  - [Launch Settings](#launch-settings)

For a better overview of the project go back to [README](https://github.com/MarioIliescu/SEP3-CSharp/blob/master/README.md)  

## File System Overview

<img width="397" height="531" alt="FileSystem" src="https://github.com/user-attachments/assets/bc52736b-9648-4783-a434-dfbe13a65e32" />

## Program code (Entry point)

```C#
using BlazorFleetApp.Components;
using BlazorFleetApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();             
builder.Services.AddServerSideBlazor();       
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//Add client
var httpClientBuilder = builder.Services.AddHttpClient<CompanyServiceClient>(client =>
{
    //MAKE SURE THE IP IS THE SAME AS THE WebAPI
    client.BaseAddress = new Uri("https://localhost:7191"); 
    client.Timeout = TimeSpan.FromSeconds(10);
});
if (builder.Environment.IsDevelopment())
{
    httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
}

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
```

## Client

```C#
using System.Net.Http;
using System.Net.Http.Json;
using ApiContracts;

namespace BlazorFleetApp.Services;

public class CompanyServiceClient
{
    private readonly HttpClient _http;

    public CompanyServiceClient(HttpClient http)
    {
        _http = http;
    }

    // Get all companies
    public async Task<List<CompanyDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<CompanyDto>>("company") ?? new List<CompanyDto>();
    }

    // Create a company
    public async Task CreateAsync(CompanyDto company)
    {
        var response = await _http.PostAsJsonAsync("company", company);
        response.EnsureSuccessStatusCode();
    }

    // Update a company
    public async Task UpdateAsync(CompanyDto company)
    {
        var response = await _http.PutAsJsonAsync("company", company);
        response.EnsureSuccessStatusCode();
    }

    // Delete a company
    public async Task DeleteAsync(string mcNumber)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"company/{mcNumber}");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
```

## Razor Component

```C#
@page "/company"
@using ApiContracts.Company
@using BlazorFleetApp.Services
@inject CompanyServiceClient CompanyService
@rendermode InteractiveServer

<h3>Companies</h3>

<p>@statusMessage</p>

<button class="btn btn-primary mb-2" @onclick="LoadCompanies">Load Companies</button>

@if (companies != null)
{
    <ul class="list-group mb-3">
        @foreach (var company in companies)
        {
            <li class="list-group-item d-flex justify-content-between align-items-center">
                <span>@company.CompanyName (MC: @company.McNumber)</span>
                <span>
                    <button class="btn btn-sm btn-warning me-1" @onclick="() => StartEdit(company)">Edit</button>
                    <button class="btn btn-sm btn-danger" @onclick="() => DeleteCompany(company.McNumber)">Delete</button>
                </span>
            </li>
        }
    </ul>
}

<h4>@(editingCompany is null ? "Add Company" : "Edit Company")</h4>
<div class="mb-3">
    <label>MC Number:</label>
    <input class="form-control" @bind="formMcNumber" />
</div>
<div class="mb-3">
    <label>Company Name:</label>
    <input class="form-control" @bind="formCompanyName" />
</div>
<button class="btn btn-success me-1" @onclick="SaveCompany">Save</button>
<button class="btn btn-secondary" @onclick="CancelEdit">Cancel</button>

@code {
    private List<CreateCompanyDto>? companies;
    private string statusMessage = "Click 'Load Companies' to fetch data.";

    private CreateCompanyDto? editingCompany = null;
    private string formMcNumber = "";
    private string formCompanyName = "";

    private async Task LoadCompanies()
    {
        try
        {
            companies = await CompanyService.GetAllAsync();
            statusMessage = $"Loaded {companies.Count} companies.";
        }
        catch (Exception ex)
        {
            statusMessage = $"Error loading companies: {ex.Message}";
        }
    }

    private void StartEdit(CreateCompanyDto company)
    {
        editingCompany = company;
        formMcNumber = company.McNumber;
        formCompanyName = company.CompanyName;
    }

    private void CancelEdit()
    {
        editingCompany = null;
        formMcNumber = "";
        formCompanyName = "";
    }

    private async Task SaveCompany()
    {
        if (string.IsNullOrWhiteSpace(formMcNumber) || string.IsNullOrWhiteSpace(formCompanyName))
        {
            statusMessage = "MC Number and Company Name are required.";
            return;
        }

        try
        {
            CreateCompanyDto? dto;
            if (editingCompany != null)
            {
                 dto = new CreateCompanyDto(editingCompany.Id, formMcNumber, formCompanyName);
            }
            else
            {
                 dto = new CreateCompanyDto(Id: 0, formMcNumber, formCompanyName);
            }

            if (editingCompany == null)
            {
                await CompanyService.CreateAsync(dto);
                statusMessage = $"Company '{dto.CompanyName}' created.";
            }
            else
            {
                await CompanyService.UpdateAsync(dto);
                statusMessage = $"Company '{dto.CompanyName}' updated.";
            }

            await LoadCompanies();
            CancelEdit();
        }
        catch (Exception ex)
        {
            statusMessage = $"Error saving company: {ex.Message}";
        }
    }

    private async Task DeleteCompany(string mcNumber)
    {
        try
        {
            await CompanyService.DeleteAsync(mcNumber);
            statusMessage = $"Company with MC '{mcNumber}' deleted.";
            await LoadCompanies();
        }
        catch (Exception ex)
        {
            statusMessage = $"Error deleting company: {ex.Message}";
        }
    }
}
```

### Navbar

Add new pages to the navbar.

```html
﻿<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">BlazorFleetApp</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler"/>

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="nav flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> Home
            </NavLink>
        </div>

        <div class="nav-item px-3">
            <NavLink class="nav-link" href="company">
                <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> Company
            </NavLink>
        </div>
    </nav>
</div>
```

### App

App.razor, can change the settings of the app and also the stylesheet.

```html
﻿@using BlazorFleetApp.Components.Layout
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <base href="/"/>
    <link rel="stylesheet" href="@Assets["lib/bootstrap/dist/css/bootstrap.min.css"]"/>
    <link rel="stylesheet" href="@Assets["app.css"]"/>
    <link rel="stylesheet" href="@Assets["BlazorFleetApp.styles.css"]"/>
    <ImportMap/>
    <link rel="icon" type="image/png" href="favicon.png"/>
    <HeadOutlet/>
</head>

<body>
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <p>Sorry, there's nothing at this address.</p>
    </NotFound>
</Router>
<script src="_framework/blazor.web.js"></script>
</body>

</html>
```

## Launch Settings

Make sure the ports are different between `WebAPI` and `BlazorClient`.  

```Json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
    "profiles": {
      "http": {
        "commandName": "Project",
        "dotnetRunMessages": true,
        "launchBrowser": true,
        "applicationUrl": "http://localhost:7031",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        }
      },
      "https": {
        "commandName": "Project",
        "dotnetRunMessages": true,
        "launchBrowser": true,
        "applicationUrl": "https://localhost:7077;http://localhost:7031",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        }
      }
    }
  }
```
