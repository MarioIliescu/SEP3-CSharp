using System.Runtime.InteropServices.JavaScript;
using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;
using Repositories;

namespace GrpcAPI.Services;

public class CompanyServiceProto : ICompanyRepository
{
    private readonly FleetMainGrpcHandler handler;

    public CompanyServiceProto(FleetMainGrpcHandler fleetMainGrpcHandler)
    {
        handler = fleetMainGrpcHandler;
    }
    
    public async Task<Company> CreateAsync(Company payload)
    {
        CompanyProto proto = new()
        {
            McNumber = payload.McNumber,
            Id = payload.Id,
            CompanyName = payload.CompanyName
        };

        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionCreate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        
        var response = await handler.SendRequestAsync(request);
        CompanyProto received = response.Payload.Unpack<CompanyProto>();
        
        return await Task.FromResult(new Company.Builder()
            .SetCompanyName(received.CompanyName)
            .SetMcNumber(received.McNumber)
            .SetId(received.Id)
            .Build());
    }
    
    public async Task UpdateAsync(Company payload)
    {
        Company updated = new Company.Builder()
            .SetCompanyName(payload.CompanyName)
            .SetMcNumber(payload.McNumber)
            .SetId(payload.Id).Build();

        CompanyProto proto = new()
        {
            CompanyName = updated.CompanyName,
            Id = updated.Id,
            McNumber = updated.McNumber
        };

        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionUpdate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        await handler.SendRequestAsync(request);
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
        var response = await handler.SendRequestAsync(request);
        CompanyProto received = response.Payload.Unpack<CompanyProto>();

        return await Task.FromResult(new Company.Builder()
            .SetCompanyName(received.CompanyName)
            .SetMcNumber(received.McNumber)
            .SetId(received.Id)
            .Build());
    }
    public async Task<Company> GetSingleAsync(int id)
    {
        CompanyProto proto = new()
        {
            Id = id
        };

        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionGet,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        var response = await handler.SendRequestAsync(request);
        CompanyProto received = response.Payload.Unpack<CompanyProto>();

        return await Task.FromResult(new Company.Builder()
            .SetCompanyName(received.CompanyName)
            .SetMcNumber(received.McNumber)
            .SetId(received.Id)
            .Build());
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
        
        await handler.SendRequestAsync(request);
    }
    public async Task DeleteAsync(int id)
    {
        CompanyProto proto = new()
        {
            Id = id
        };
        
        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionDelete,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        
        await handler.SendRequestAsync(request);
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
                McNumber = "default"
            })
        };

        var response = handler.SendRequestAsync(request);
        CompanyProtoList received = response.Result.Payload.Unpack<CompanyProtoList>();

        List<Company> companies = new();

        foreach (CompanyProto company in received.Companies)
        {
            companies.Add(new Company.Builder()
                .SetCompanyName(company.CompanyName)
                .SetMcNumber(company.McNumber)
                .SetId(company.Id)
                .Build());
        }
        return companies.AsQueryable();
    }
}
