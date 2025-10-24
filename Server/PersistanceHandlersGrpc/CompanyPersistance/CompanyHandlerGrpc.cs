using System.ComponentModel;
using System.Reflection;
using ApiContracts;
using ApiContracts.Enums;
using GrpcAPI.Services;
using Entities;
using PersistanceContracts;

namespace PersistanceHandlersGrpc.CompanyPersistance;

public class CompanyHandlerGrpc : IFleetPersistanceHandler
{
    private readonly CompanyServiceProto _companyService ;
    public CompanyHandlerGrpc(CompanyServiceProto _companyService)
    {
        this._companyService = _companyService;
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
                await HandleDeleteAsync(company);;
                break;
            }
            case ActionType.Get:
            {
                return await HandleGetAsync(company);
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

    private async Task<Company> HandleGetAsync(Company company)
    {
        if (company.Id != 0)
        {
            return await _companyService.GetSingleAsync(company.Id);
        }

        if (!string.IsNullOrEmpty(company.McNumber))
        {
            return await _companyService.GetSingleAsync(company.McNumber);
        }

        throw new ArgumentException("Invalid company");
    }

    private async Task HandleDeleteAsync(Company company)
    {
        if (company.Id != 0)
        {
            await _companyService.DeleteAsync(company.Id);
        }

        if (!string.IsNullOrEmpty(company.McNumber))
        {
            await _companyService.DeleteAsync(company.McNumber);
        }
    }
}