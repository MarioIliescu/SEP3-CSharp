using Entities;
using GrpcAPI.GrpcUtils;
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
        _logger.LogInformation("Creating new company");
        try
        {
            var response = await handler.SendRequestAsync(
                ProtoUtils.ParseCompanyRequest(ActionTypeProto.ActionCreate, payload));
            CompanyProto received = response.Payload.Unpack<CompanyProto>();
            _logger.LogInformation($"Created new company {received.McNumber}");
            return await Task.FromResult(ProtoUtils.ParseFromProtoToCompany(received));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateAsync(Company payload)
    {
        try
        {
            await handler.SendRequestAsync(ProtoUtils.ParseCompanyRequest(ActionTypeProto.ActionUpdate, payload));
            _logger.LogInformation($"Updated {payload.McNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company");
        }
    }

    public async Task<Company> GetSingleAsync(string mcNumber)
    {
        try
        {
            var response = await handler.SendRequestAsync(
                ProtoUtils.ParseCompanyRequest(
                    ActionTypeProto.ActionGet,
                    new Company.Builder()
                        .SetMcNumber(mcNumber)
                        .Build()));
            CompanyProto received = response.Payload.Unpack<CompanyProto>();
            _logger.LogInformation($"Company received, {received.McNumber}");
            return await Task.FromResult(ProtoUtils
                .ParseFromProtoToCompany(received));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company");
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteAsync(string mcNumber)
    {
        try
        {
            await handler.SendRequestAsync(
                ProtoUtils.ParseCompanyRequest(
                    ActionTypeProto.ActionDelete,
                    new Company.Builder()
                        .SetMcNumber(mcNumber)
                        .Build()));
            _logger.LogInformation($"Deleted company {mcNumber}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting company");
            throw new Exception(e.Message);
        }
    }

    public IQueryable<Company> GetManyAsync()
    {
        try
        {
            var response = handler.SendRequestAsync(
                ProtoUtils.ParseCompanyRequest(
                    ActionTypeProto.ActionList,
                    //Do not leave empty
                    new Company.Builder()
                        .SetCompanyName("default")
                        .SetMcNumber("DEFAULTMCN")
                        .Build()));
            CompanyProtoList received = response.Result.Payload.Unpack<CompanyProtoList>();

            List<Company> companies = new();

            foreach (CompanyProto company in received.Companies)
            {
                companies.Add(
                    ProtoUtils.ParseFromProtoToCompany(company));
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