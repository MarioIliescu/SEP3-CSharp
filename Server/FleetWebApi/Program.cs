using Grpc.Net.Client;
using GrpcAPI;
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
builder.Services.AddSingleton<FleetMainGrpcHandler>(sp =>
{
    var channel =
        Grpc.Net.Client.GrpcChannel.ForAddress("http://localhost:6032");
    return new FleetMainGrpcHandler(channel);
});
var app = builder.Build();
app.UseRouting();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();