using BlazorFleetApp.Authentification;
using BlazorFleetApp.Components;
using BlazorFleetApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorFleetApp.Services.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();             
builder.Services.AddServerSideBlazor();       
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient()
{
    BaseAddress = new Uri("https://localhost:7191")
});

builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();

if (builder.Environment.IsDevelopment())
{
    httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
}

builder.Services.AddScoped<ICompanyService, CompanyServiceClient>();
builder.Services.AddScoped<IDriverService, DriverServiceClient>();
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