using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using ApiContracts.Enums;
using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.GrpcUtils;
using GrpcAPI.Protos;
using Microsoft.Extensions.Logging;
using Repositories;

namespace GrpcAPI.Services;

public class DriverServiceProto : IDriverRepository
{
    private readonly ILogger<DriverServiceProto> _logger;
    private readonly FleetMainGrpcHandler _fleetMainGrpcHandler;
    public DriverServiceProto(ILogger<DriverServiceProto> logger, FleetMainGrpcHandler fleetMainGrpcHandler)
    {
        _logger = logger;
        _fleetMainGrpcHandler = fleetMainGrpcHandler;
    }
    
    public async Task<Driver> CreateAsync(Driver payload)
    {
        try
        {
            _logger.LogInformation("Creating new user");
            var response = await _fleetMainGrpcHandler.SendRequestAsync(
                ProtoUtils.ParseDriverRequest(ActionTypeProto.ActionCreate, payload));
            var createdProto = response.Payload.Unpack<DriverProto>();
            var created = ProtoUtils.ParseFromProtoToDriver(createdProto);
            _logger.LogInformation($"Created new user with Id {created.Id}");
            return await Task.FromResult(created);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating new user");
            throw new Exception(e.Message);
        }
        
    }

    public async Task UpdateAsync(Driver payload)
    {
        try
        {
            _logger.LogInformation("Updating user");
            await _fleetMainGrpcHandler.SendRequestAsync(
                ProtoUtils.ParseDriverRequest(ActionTypeProto.ActionUpdate, payload));
            _logger.LogInformation($"Updated new user with Id {payload.Id}");
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
        
    }

    public async Task<Driver> GetSingleAsync(int id)
    {
        _logger.LogInformation("Getting single user");
        try
        {
            var response = await _fleetMainGrpcHandler.SendRequestAsync(
                ProtoUtils.ParseDriverRequest(ActionTypeProto.ActionGet,
                new Driver
                        .Builder()
                    .SetId(id)
                    .Build()));
            var driverProto = response.Payload.Unpack<DriverProto>();
            var driver = ProtoUtils.ParseFromProtoToDriver(driverProto);
            _logger.LogInformation($"Getting single user with Id {id}");
            return await Task.FromResult(driver);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting driver");
        try
        {
            await _fleetMainGrpcHandler.SendRequestAsync(ProtoUtils.ParseDriverRequest(ActionTypeProto.ActionDelete,
                new Driver
                        .Builder()
                    .SetId(id)
                    .Build()));
            _logger.LogInformation($"Deleted driver with Id {id}");
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
        
    }

    public IQueryable<Driver> GetManyAsync()
    {
        _logger.LogInformation("Getting all drivers");
        RequestProto request = new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerDriver,
            Action = ActionTypeProto.ActionList,
            Payload = Any.Pack(new DriverProto()
            {
                User = new UserProto(){Id = 0},
            })
        };
        try
        {
            var response = _fleetMainGrpcHandler.SendRequestAsync(request);
            DriverListProto received = response.Result.Payload.Unpack<DriverListProto>();
            List<Driver> drivers = new();
            foreach (DriverProto driver in received.Drivers)
            {
                drivers.Add(ProtoUtils.ParseFromProtoToDriver(driver));
            }

            return drivers.AsQueryable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception(ex.Message);
        }
    }
}