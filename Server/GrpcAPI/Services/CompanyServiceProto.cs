using System.Runtime.InteropServices.JavaScript;
using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;
using Microsoft.Extensions.Logging;
using Repositories;

namespace GrpcAPI.Services;

public class CompanyServiceProto : ICompanyRepository
{
    private readonly FleetMainGrpcHandler handler;
    private readonly ILogger<CompanyServiceProto> _logger;
    public CompanyServiceProto(FleetMainGrpcHandler fleetMainGrpcHandler, ILogger<CompanyServiceProto> logger)
    {
        handler = fleetMainGrpcHandler;
        _logger = logger;
    }
    
    public async Task<Company> CreateAsync(Company payload)
    {
        CompanyProto proto = new()
        {
            McNumber = payload.McNumber,
            CompanyName = payload.CompanyName
        };

        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionCreate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        _logger.LogInformation("Creating new company");
        try
        {
            var response = await handler.SendRequestAsync(request);
            _logger.LogInformation($"Created new company {proto.McNumber}");
            CompanyProto received = response.Payload.Unpack<CompanyProto>();
            return await Task.FromResult(new Company.Builder()
                .SetCompanyName(received.CompanyName)
                .SetMcNumber(received.McNumber)
                .Build());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            throw new Exception(ex.Message);
        }

    }
    
    public async Task UpdateAsync(Company payload)
    {
        Company updated = new Company.Builder()
            .SetCompanyName(payload.CompanyName)
            .SetMcNumber(payload.McNumber)
            .Build();

        CompanyProto proto = new()
        {
            CompanyName = updated.CompanyName,
            McNumber = updated.McNumber
        };

        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionUpdate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        _logger.LogInformation($"Updating company{proto.McNumber}");
        try
        {
            await handler.SendRequestAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company");
        }
    }
    public async Task<Company> GetSingleAsync(string mcNumber)
    {
        CompanyProto proto = new()
        {
            McNumber = mcNumber
        };

        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionGet,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        _logger.LogInformation($"Getting company {proto.McNumber}");
        try
        {
            var response = await handler.SendRequestAsync(request);
            CompanyProto received = response.Payload.Unpack<CompanyProto>();
            _logger.LogInformation($"Company received, {received.McNumber}");
            return await Task.FromResult(new Company.Builder()
                .SetCompanyName(received.CompanyName)
                .SetMcNumber(received.McNumber)
                .Build());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company");
            throw new Exception(ex.Message);
        }
    }
    
    public async Task DeleteAsync(string mcNumber)
    {
        CompanyProto proto = new()
        {
            McNumber = mcNumber
        };
        
        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionDelete,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        _logger.LogInformation($"Deleting company {proto.McNumber}");
        try
        {
            await handler.SendRequestAsync(request);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting company");
            throw new Exception(e.Message);
        }
        
    }
    public IQueryable<Company> GetManyAsync()
    {
        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionList,
            Handler = HandlerTypeProto.HandlerCompany,
            Payload = Any.Pack(new CompanyProto()
            //Must put in a payload, do not leave null otherwise there will be an exception on the Java server
            {
                CompanyName = "default",
                McNumber = "default123"
            })
        };
        _logger.LogInformation($"Getting all company");
        try
        {
            var response = handler.SendRequestAsync(request);
            CompanyProtoList received = response.Result.Payload.Unpack<CompanyProtoList>();

            List<Company> companies = new();

            foreach (CompanyProto company in received.Companies)
            {
                companies.Add(new Company.Builder()
                    .SetCompanyName(company.CompanyName)
                    .SetMcNumber(company.McNumber)
                    .Build());
            }

            _logger.LogInformation($"Companies returned {received.Companies.Count}");
            return companies.AsQueryable();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting all company");
            throw new Exception(e.Message);
        }
    }
}
