using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using ApiContracts.Enums;
using Entities;
using Google.Protobuf.WellKnownTypes;
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
                ParseDriverRequest(ActionTypeProto.ActionCreate, payload));
            var createdProto = response.Payload.Unpack<DriverProto>();
            var created = ParseFromProtoToDriver(createdProto);
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
                ParseDriverRequest(ActionTypeProto.ActionUpdate, payload));
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
            var response = await _fleetMainGrpcHandler.SendRequestAsync(ParseDriverRequest(ActionTypeProto.ActionGet,
                new Driver
                        .Builder()
                    .SetId(id)
                    .Build()));
            var driverProto = response.Payload.Unpack<DriverProto>();
            var driver = ParseFromProtoToDriver(driverProto);
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
            await _fleetMainGrpcHandler.SendRequestAsync(ParseDriverRequest(ActionTypeProto.ActionDelete,
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
                drivers.Add(ParseFromProtoToDriver(driver));
            }

            return drivers.AsQueryable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception(ex.Message);
        }
    }

    //Helper methods
    private Driver ParseFromProtoToDriver(DriverProto proto)
    {
        return new Driver.Builder()
            .SetId(proto.User.Id)
            .SetLocationState(proto.CurrentState)
            .SetLocationZip(proto.CurrentZIPCODE)
            .SetTrailerType(ParseTrailerTypeProtoToTrailerType(proto.TrailerType))
            .SetCompanyRole(ParseFromDriverCompanyRoleProtoToCompanyRole(proto.CompanyRole))
            .SetStatus(ParseDriverStatusProtoToDriverStatus(proto.DriverStatus))
            .SetMcNumber(proto.CompanyMcNumber)
            .SetEmail(proto.User.Email)
            .SetPassword(proto.User.Password)
            .SetFirstName(proto.User.FirstName)
            .SetLastName(proto.User.LastName)
            .SetRole(UserRole.DRIVER)
            .Build();
    }

    private DriverStatus ParseDriverStatusProtoToDriverStatus(StatusDriverProto proto)
    {
        switch (proto)
        {
            case StatusDriverProto.Available:
                return DriverStatus.available;
            case StatusDriverProto.Busy:
                return DriverStatus.busy;
            case StatusDriverProto.OffDuty:
                return DriverStatus.off_duty;
            default:
                _logger.LogError($"Unknown status driver {proto}");
                throw new InvalidEnumArgumentException("Invalid status driver status");
        }
    }

    private DriverCompanyRole ParseFromDriverCompanyRoleProtoToCompanyRole(DriverCompanyRoleProto proto)
    {
        switch (proto)
        {
            case DriverCompanyRoleProto.Driver:
                return DriverCompanyRole.Driver;
            case DriverCompanyRoleProto.OwnerOperator:
                return DriverCompanyRole.OwnerOperator;
            default:
                _logger.LogError($"Unknown company role {proto}");
                throw new InvalidEnumArgumentException("Invalid driver company role");
        }
    }

    private TrailerType ParseTrailerTypeProtoToTrailerType(TrailerTypeProto trailerType)
    {
        switch (trailerType)
        {
            case TrailerTypeProto.DryVan:
                return TrailerType.dry_van;
            case TrailerTypeProto.Flatbed:
                return TrailerType.flatbed;
            case TrailerTypeProto.Reefer:
                return TrailerType.reefer;
            default:
                _logger.LogError($"Unknown trailer type {trailerType}");
                throw new InvalidEnumArgumentException("Unknown trailer type");
        }
    }

    private RequestProto ParseDriverRequest(ActionTypeProto action, Driver payload)
    {
        return new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerDriver,
            Action = action,
            Payload = Any.Pack(ParseDriverToProto(payload))
        };
    }

    private DriverProto ParseDriverToProto(Driver payload)
    {
        int zipcode = 35010;
        if (payload.Location_Zip_Code == 0)
        {
            zipcode = payload.Location_Zip_Code;
        }
        return new DriverProto()
        {
            User = ParseUserToProto(payload),
            CompanyMcNumber = payload.McNumber ?? "DEFAULTMCN",
            CurrentState = payload.Location_State ?? "",
            DriverStatus = ParseStatusToProto(payload.Status),
            CompanyRole = ParseCompanyRoleToProto(payload.CompanyRole),
            TrailerType = ParseTrailerTypeToProto(payload.Trailer_type),
            CurrentZIPCODE = zipcode 
        };
    }

    private UserProto ParseUserToProto(User user)
    {
        return new UserProto()
        {
            Id = user.Id,
            Email = user.Email ?? "default@default.com",
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            PhoneNumber = user.PhoneNumber ?? "+4511111111",
            Password = user.Password ?? "VXe6FQmH2*UAQu9U7&wTnD1x7ERS@w*RahW*",
        };
    }

    private StatusDriverProto ParseStatusToProto(DriverStatus status)
    {
        switch (status)
        {
            case DriverStatus.available:
                return StatusDriverProto.Available;
            case DriverStatus.busy:
                return StatusDriverProto.Busy;
            case DriverStatus.off_duty:
                return StatusDriverProto.OffDuty;
            default:
                _logger.LogError($"Unknown status driver {status}");
                throw new InvalidEnumArgumentException("Status unknown");
        }
    }

    private DriverCompanyRoleProto ParseCompanyRoleToProto(DriverCompanyRole payload)
    {
        switch (payload)
        {
            case DriverCompanyRole.Driver:
                return DriverCompanyRoleProto.Driver;
            case DriverCompanyRole.OwnerOperator:
                return DriverCompanyRoleProto.OwnerOperator;
            default:
                _logger.LogError($"Unknown company role {payload}");
                throw new InvalidEnumArgumentException("Unknown company role");
        }
    }

    private TrailerTypeProto ParseTrailerTypeToProto(TrailerType trailerType)
    {
        switch (trailerType)
        {
            case TrailerType.dry_van:
                return TrailerTypeProto.DryVan;
            case TrailerType.flatbed:
                return TrailerTypeProto.Flatbed;
            case TrailerType.reefer:
                return TrailerTypeProto.Reefer;
            default:
                _logger.LogError($"Unknown trailer type {trailerType}");
                throw new InvalidEnumArgumentException("Unknown trailer type");
        }
    }
}