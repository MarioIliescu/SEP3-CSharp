using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using GrpcAPI.Services;
using Entities;

using PersistanceContracts;

namespace PersistanceHandlersGrpc.CompanyPersistance;

public class CompanyHandlerGrpc : IFleetPersistanceHandler
{
    private readonly CompanyServiceProto _companyService ;
    public CompanyHandlerGrpc(CompanyServiceProto companyService)
    {
        this._companyService = companyService;
    }

    public async Task<object> HandleAsync(Request request)
    {
        var company = (Company)request.Payload?? throw new ArgumentNullException(nameof(request.Payload));
        switch (request.Action)
        {
            case ActionType.Create:
            {
                return await _companyService.CreateAsync(company);
            }
            case ActionType.Update:
            {
                await _companyService.UpdateAsync(company);
                break;
            }
            case ActionType.Delete:
            {
                await _companyService.DeleteAsync(company.McNumber);
                break;
            }
            case ActionType.Get:
            {
                return await _companyService.GetSingleAsync(company.McNumber);
            }
            case ActionType.List:
            {
                return _companyService.GetManyAsync();
            }
            default:
                throw new InvalidEnumArgumentException("Unknown action type");
        }
        return Task.CompletedTask;
    }
}