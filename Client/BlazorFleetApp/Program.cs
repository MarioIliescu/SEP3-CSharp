using BlazorFleetApp.Authentication;
using BlazorFleetApp.Components;
using BlazorFleetApp.Services;
using BlazorFleetApp.Services.Auth;
using BlazorFleetApp.Services.Dispatcher;
using BlazorFleetApp.Services.Driver;
using BlazorFleetApp.Services.Job;
using BlazorFleetApp.Services.RecruitDriver;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient()
    { BaseAddress = new Uri(builder.Configuration["FleetWebApi:BaseAddress"] ?? "") });
builder.Services.AddAuthentication().AddCookie(options => { options.LoginPath = "/login"; });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IDriverService, DriverServiceClient>();
builder.Services.AddScoped<ICompanyService, CompanyServiceClient>();
builder.Services.AddScoped<IDispatcherService, DispatcherServiceClient>();
builder.Services.AddScoped<IJobService, JobServiceClient>();
builder.Services.AddScoped<IRecruitService, RecruitService>();
builder.Services.AddScoped<IAuthService, JwtAuthService>();
if (builder.Environment.IsDevelopment())
{
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
