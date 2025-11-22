using BlazorFleetApp.Authentification;
using BlazorFleetApp.Components;
using BlazorFleetApp.Services;
using BlazorFleetApp.Services.Driver;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var httpClientBuilder = builder.Services.AddHttpClient<CompanyServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7191");
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IDriverService, DriverServiceClient>();
builder.Services.AddScoped<ICompanyService, CompanyServiceClient>();

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