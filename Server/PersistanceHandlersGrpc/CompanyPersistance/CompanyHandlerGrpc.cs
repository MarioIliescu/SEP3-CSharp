using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using GrpcAPI.Services;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace PersistanceHandlersGrpc.CompanyPersistance;

public class CompanyHandlerGrpc : IFleetPersistanceHandler
{
    private readonly ICompanyRepository _companyService ;
    private readonly ILogger<CompanyHandlerGrpc> _logger;
    public CompanyHandlerGrpc([FromKeyedServices(HandlerType.Company)]ICompanyRepository companyService, ILogger<CompanyHandlerGrpc> logger)
    {
        this._companyService = companyService;
        this._logger = logger;
    }

    public async Task<object> HandleAsync(Request request)
    {
        try
        {
            var company = (Company)request.Payload ?? throw new ArgumentNullException(nameof(request.Payload));
            switch (request.Action)
            {
                case ActionType.Create:
                {
                    _logger.LogInformation($"Created company {company}");
                    return await _companyService.CreateAsync(company);
                }
                case ActionType.Update:
                {
                    await _companyService.UpdateAsync(company);
                    _logger.LogInformation($"Updated company {company}");
                    break;
                }
                case ActionType.Delete:
                {
                    await _companyService.DeleteAsync(company.McNumber);
                    _logger.LogInformation($"Deleted company {company.McNumber}");
                    break;
                }
                case ActionType.Get:
                {
                    _logger.LogInformation($"Get company {company.McNumber}");
                    return await _companyService.GetSingleAsync(company.McNumber);
                }
                case ActionType.List:
                {
                    _logger.LogInformation($"List company {company.McNumber}");
                    return _companyService.GetManyAsync();
                }
                default:
                    _logger.LogError($"Unknown action {request.Action}");
                    throw new InvalidEnumArgumentException("Unknown action type");
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}