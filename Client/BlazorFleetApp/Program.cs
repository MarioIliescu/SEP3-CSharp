using BlazorFleetApp.Authentification;
using BlazorFleetApp.Components;
using BlazorFleetApp.Services;
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
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IDriverService, DriverServiceClient>();
builder.Services.AddScoped<ICompanyService, CompanyServiceClient>();

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

app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
