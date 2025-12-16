using BlazorFleetApp.Authentication;
using BlazorFleetApp.Components;
using BlazorFleetApp.Services;
using BlazorFleetApp.Services.Auth;
using BlazorFleetApp.Services.Dispatcher;
using BlazorFleetApp.Services.Driver;
using BlazorFleetApp.Services.Events;
using BlazorFleetApp.Services.Hubs;
using BlazorFleetApp.Services.Job;
using BlazorFleetApp.Services.RecruitDriver;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["FleetWebApi:BaseAddress"] ?? "")
});
builder.Services.AddAuthentication().AddCookie(options => { options.LoginPath = "/login"; });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<IDriverService, DriverServiceClient>();
builder.Services.AddScoped<ICompanyService, CompanyServiceClient>();
builder.Services.AddScoped<IDispatcherService, DispatcherServiceClient>();
builder.Services.AddScoped<IJobService, JobServiceClient>();
builder.Services.AddScoped<IRecruitService, RecruitService>();
builder.Services.AddScoped<IAuthService, JwtAuthService>();
builder.Services.AddScoped<FleetHubClient>();
builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        [ "application/octet-stream" ]);
});
if (builder.Environment.IsDevelopment())
{
}

var app = builder.Build();
app.UseResponseCompression();
// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chathub");
app.MapHub<FleetHub>("/fleethub");
app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
