using System.Text;
using ApiContracts.Enums;
using GrpcAPI;
using Serilog;
using GrpcAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PersistanceContracts;
using PersistanceHandlersGrpc.AuthPersistance;
using PersistanceHandlersGrpc.CompanyPersistance;
using PersistanceHandlersGrpc.UserPersistance;
using Repositories;
using Services.Auth;
using Services.Company;
using Services.Dispatcher;
using Services.Driver;

var builder = WebApplication.CreateBuilder(args);
//Add controllers to the container 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//add more services
builder.Services.AddKeyedScoped<ICompanyRepository, CompanyServiceProto>(HandlerType.Company);
builder.Services.AddKeyedScoped<IAuthRepository, AuthServiceProto>(HandlerType.Auth);
builder.Services.AddKeyedScoped<IFleetPersistanceHandler, CompanyHandlerGrpc>(HandlerType.Company);
builder.Services.AddKeyedScoped<IFleetPersistanceHandler, AuthHandlerGrpc>(HandlerType.Auth);
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IDispatcherService, DispatcherService>();
builder.Services.AddKeyedScoped<IFleetPersistanceHandler, DriverHandlerGrpc>(HandlerType.Driver);
builder.Services.AddKeyedScoped<IFleetPersistanceHandler, DispatcherHandlerGrpc>(HandlerType.Dispatcher);
builder.Services.AddKeyedScoped<IDriverRepository, DriverServiceProto>(HandlerType.Driver);
builder.Services.AddKeyedScoped<IDispatcherRepository, DispatcherServiceProto>(HandlerType.Dispatcher);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<FleetMainGrpcHandler>(sp =>
{
    var channel =
        Grpc.Net.Client.GrpcChannel.ForAddress("http://localhost:6032");
    return new FleetMainGrpcHandler(channel);
});
Directory.CreateDirectory("Logs");

// Configure Serilog
builder.Host.UseSerilog((ctx, services, lc) => lc
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/myapp-.txt",
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
     options.MapInboundClaims = false;
     options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
       ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
         ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
        ClockSkew = TimeSpan.Zero,
    }; 
});

var app = builder.Build();
app.UseRouting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();