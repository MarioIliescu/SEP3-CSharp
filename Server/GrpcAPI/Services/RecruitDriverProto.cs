using System.Runtime.InteropServices.JavaScript;
using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.GrpcUtils;
using GrpcAPI.Protos;
using Microsoft.Extensions.Logging;
using Repositories;

namespace GrpcAPI.Services;

public class RecruitDriverProto : IRecruitDriverRepository
{
    private readonly ILogger<RecruitDriverProto> _logger;
    private readonly FleetMainGrpcHandler _handler;

    public RecruitDriverProto(ILogger<RecruitDriverProto> logger, FleetMainGrpcHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    public async Task<Driver> RecruitDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        _logger.LogInformation($"Recruiting driver {driver.Id} for dispatcher {dispatcher.Id}");
        var payload = new RecruitDriver()
        {
            Driver = driver,
            Dispatcher = dispatcher
        };
        try
        {
            var response =
                await _handler.SendRequestAsync(
                    ProtoUtils.ParseRecruitDriverRequest(ActionTypeProto.ActionCreate, payload));
            Protos.RecruitDriverProto received = response.Payload.Unpack<Protos.RecruitDriverProto>();
            Protos.DriverProto receivedDriver = received.Driver;
            _logger.LogInformation($"Recruited driver {received.Driver.User.Id} for dispatcher {received.Dispatcher.User.Id}");
            return await Task.FromResult(ProtoUtils.ParseFromProtoToDriver(receivedDriver));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during recruiting driver {driver.Id} for dispatcher {dispatcher.Id}");
            throw new Exception(ex.Message);
        }
    }

    public async Task FireDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        _logger.LogInformation($"Firing driver {driver.Id} from dispatcher {dispatcher.Id}");
        var payload = new RecruitDriver()
        {
            Driver = driver,
            Dispatcher = dispatcher
        };
        try
        {
            await _handler.SendRequestAsync(
                ProtoUtils.ParseRecruitDriverRequest(ActionTypeProto.ActionDelete, payload));
            _logger.LogInformation($"Successfully fired driver {driver.Id} from dispatcher {dispatcher.Id}");
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"Error during firing driver {driver.Id} from dispatcher {dispatcher.Id}");
            throw new Exception(e.Message);
        }
    }

    public IQueryable<Driver> GetDispatcherDriversListAsync(int id)
    {
        _logger.LogInformation($"Getting all drivers under dispatcher {id}");
        RequestProto request = new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerRecruit,
            Action = ActionTypeProto.ActionGet,
            Payload = Any.Pack(new Protos.RecruitDriverProto()
            {
                Driver = new DriverProto(),
                Dispatcher = new DispatcherProto()
                {
                    User = new UserProto()
                    {
                        Id = id
                    }
                }
            })
        };

        try
        {
            var response = _handler.SendRequestAsync(request);
            DriverListProto received = response.Result.Payload.Unpack<DriverListProto>();

            List<Driver> drivers = new List<Driver>();

            foreach (DriverProto dP in received.Drivers)
            {
                drivers.Add(ProtoUtils.ParseFromProtoToDriver(dP));
            }
            
            _logger.LogInformation($"Successfully returned {received.Drivers.Count} drivers");
            return drivers.AsQueryable();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error during getting all drivers under dispatcher {id}");
            throw  new Exception(e.Message);
        }
    }

    public IQueryable<Driver> GetDriverListWoDispatcherAsync()
    {
        _logger.LogInformation("Getting all drivers without a dispatcher");
        RequestProto requestProto = new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerRecruit,
            Action = ActionTypeProto.ActionList,
            Payload = Any.Pack(new Protos.RecruitDriverProto(){})
        };

        try
        {
            var response = _handler.SendRequestAsync(requestProto);
            DriverListProto received = response.Result.Payload.Unpack<DriverListProto>();
            
            List<Driver> drivers = new List<Driver>();

            foreach (DriverProto dP in received.Drivers)
            {
                drivers.Add(ProtoUtils.ParseFromProtoToDriver(dP));
            }
            
            _logger.LogInformation($"Successfully returned {received.Drivers.Count} drivers");
            return drivers.AsQueryable();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during getting the drivers without a dispatchers");
            throw new Exception(e.Message);
        }
    }
}