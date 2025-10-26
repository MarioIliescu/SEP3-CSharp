# Grpc API  

- [Grpc API](#grpc-api)
  - [Nuget packages IMPORTANT](#nuget-packages-important)
  - [Required folder structure IMPORTANT](#required-folder-structure-important)
  - [Proto file IMPORTANT](#proto-file-important)
  - [Grpc Handlers](#grpc-handlers)
    - [CompanyHandlerGrpc](#companyhandlergrpc)
  - [Services](#services)
    - [CompanyServiceProto](#companyserviceproto)
      - [Create](#create)
      - [Update](#update)
      - [Get Single](#get-single)
      - [Delete](#delete)
      - [GetMany](#getmany)
  - [MainHandler with Grpc Client](#mainhandler-with-grpc-client)

For a better overview of the project go back to [README](https://github.com/MarioIliescu/SEP3-CSharp/blob/master/README.md)  

## Nuget packages IMPORTANT

<img width="389" height="177" alt="GrpcNugets" src="https://github.com/user-attachments/assets/27e40164-4e2c-4e02-ac7d-40107290034c" />

## Required folder structure IMPORTANT

<img width="388" height="299" alt="folder structure" src="https://github.com/user-attachments/assets/bbadd9d3-cb97-4233-88e2-15dad748de5e" />

## Proto file IMPORTANT

MUST BE THE SAME ON BOTH SIDES BESIDES:  

```protobuf
option csharp_namespace = "GrpcAPI.Protos";
```

For C#

## Grpc Handlers  

All handlers implement `IFleetPersistanceHandler` found in `README`.  

Are used to handle persistance of objects transmited through Grpc to the Database layer.  

Example broken down by steps:  

### CompanyHandlerGrpc

Implement the interface and create a `readonly` instance of the designated service.
Create a constructor that takes the implementation of the service.

```C#
namespace PersistanceHandlersGrpc.CompanyPersistance;

public class CompanyHandlerGrpc : IFleetPersistanceHandler
{
    private readonly CompanyServiceProto _companyService ;
    public CompanyHandlerGrpc(CompanyServiceProto _companyService)
    {
        this._companyService = _companyService;
    }
```

Create the HandleAsync method:  

```C#
public async Task<object> HandleAsync(Request request)
    {
        //If the payload exists, otherwise throw exception
        //Cast it to company
        var company = (Company)request.Payload?? throw new ArgumentNullException(nameof(request.Payload));
        //Switch depending on the action
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
                //helper method
                return await HandleGetAsync(company);
            }
            case ActionType.List:
            {
                //helper method
                return _companyService.GetManyAsync();
            }
            default:
                throw new InvalidEnumArgumentException("Unknown action type");
        }
        //if nothing was returned and no exception was thrown
        //In case of Update or Delete
        return Task.CompletedTask;
    }
```

Helper methods

```C#
 private async Task<Company> HandleGetAsync(Company company)
    {
        //2 types of get handled in one line
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
        //2 types of delete handled in one line
        if (company.Id != 0)
        {
            await _companyService.DeleteAsync(company.Id);
        }

        if (!string.IsNullOrEmpty(company.McNumber))
        {
            await _companyService.DeleteAsync(company.McNumber);
        }
    }
```

## Services

Used to transform `Entities` into `Grpc` transferable `objects`.

### CompanyServiceProto

```C#
namespace GrpcAPI.Services;
//Implements repository because it uses CRUD operations
public class CompanyServiceProto : ICompanyRepository
{
    //singleton connection to the database that sends requests to the Java Server
    private readonly FleetMainGrpcHandler handler = FleetMainGrpcHandler.Instance;
}
```

#### Create  

```C#
   public async Task<Company> CreateAsync(Company payload)
    {
        //Create the proto object
        CompanyProto proto = new()
        {
            McNumber = payload.McNumber,
            Id = payload.Id,
            CompanyName = payload.CompanyName
        };
        //Create the request with the action payload and handler
        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionCreate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        // Send the request and get the response
        var response = await handler.SendRequestAsync(request);
        //Unpack the object from the response
        CompanyProto received = response.Payload.Unpack<CompanyProto>();
        //Return an instance of Company Entity
        return await Task.FromResult(new Company.Builder()
            .SetCompanyName(received.CompanyName)
            .SetMcNumber(received.McNumber)
            .SetId(received.Id)
            .Build());
    }
```

#### Update

```C#
 public async Task UpdateAsync(Company payload)
    {
        //Get the object needed from the database
        Company companyToUpdate = await GetSingleAsync(payload.Id);
        //Change the parameteres with the ones in the payload
        Company updated = new Company.Builder()
            .SetCompanyName(payload.CompanyName)
            .SetMcNumber(payload.McNumber)
            .SetId(payload.Id).Build();
            //Create the proto payload
        CompanyProto proto = new()
        {
            CompanyName = updated.CompanyName,
            Id = updated.Id,
            McNumber = updated.McNumber
        };
        //Create the request and send it
        RequestProto request = new()
        {
            Action = ActionTypeProto.ActionUpdate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerCompany
        };
        await handler.SendRequestAsync(request);
    }
```

#### Get Single

2 implementations depending on the payload information

```C#
//Either McNumber or Id is provided 
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
```

#### Delete

```C#
//either id or mcNumber
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
```

#### GetMany

```C#
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
        //get response with the list
        var response = handler.SendRequestAsync(request);
        //Unpack the reponse
        CompanyProtoList received = response.Result.Payload.Unpack<CompanyProtoList>();
        //Make a new list
        List<Company> companies = new();
        //For each object in the list make a new Entity of Company 
        foreach (CompanyProto company in received.Companies)
        {
            companies.Add(new Company.Builder()
                .SetCompanyName(company.CompanyName)
                .SetMcNumber(company.McNumber)
                .SetId(company.Id)
                .Build());
        }
        //Return as Queryable
        return companies.AsQueryable();
    }
```

---

## MainHandler with Grpc Client

Add this to protobuf

```protobuf
service FleetServiceProto {
  rpc SendRequest (RequestProto) returns (ResponseProto);
}
```

C# code

```C#
using Grpc.Net.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;

namespace GrpcAPI
{
    public class FleetMainGrpcHandler
    {
        //Service in proto file
        //Grpc Client
        private readonly FleetServiceProto.FleetServiceProtoClient _client;
        private const string Host = "http://localhost:6032";
        //Singleton
        private static readonly Lazy<FleetMainGrpcHandler> _instance =
            new Lazy<FleetMainGrpcHandler>(() =>
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress(Host);
                return new FleetMainGrpcHandler(channel);
            });
        
        private FleetMainGrpcHandler(GrpcChannel channel)
        {
            _client = new FleetServiceProto.FleetServiceProtoClient(channel);
        }
        //Get the instance
        public static FleetMainGrpcHandler Instance => _instance.Value;
        
        /// <summary>
        /// Sends a request to the gRPC server specifying the handler and action
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The server response</returns>
        public async Task<ResponseProto> SendRequestAsync(RequestProto request)
        {
            try
            { 
                //Try the connection and send it to the Ip specified
                ResponseProto  response= await _client.SendRequestAsync(request);
                Console.WriteLine(response);
                return  response;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"RPC failed: {ex.Status}");
                throw;
            }
        }
    }
}
```
