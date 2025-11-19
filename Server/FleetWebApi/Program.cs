using ApiContracts.Enums;
using Grpc.Net.Client;
using GrpcAPI;
using Serilog;
using Serilog.Events;
using GrpcAPI.Services;
using PersistanceContracts;
using PersistanceHandlersGrpc.CompanyPersistance;
using Repositories;
using Services.Company;

var builder = WebApplication.CreateBuilder(args);
//Add controllers to the container 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//add more services
builder.Services.AddKeyedScoped<ICompanyRepository,CompanyServiceProto>("companyProto");
builder.Services.AddKeyedScoped<IFleetPersistanceHandler,CompanyHandlerGrpc>(HandlerType.Company);
builder.Services.AddScoped<ICompanyService, CompanyService>();
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
var app = builder.Build();
app.UseRouting();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();